// -----------------------------------------------------------------------
// <copyright file="AuditRecordConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.ResourceConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.PartnerCenter.AuditRecords;
    using Models.PartnerCenter.Customers;
    using Models.PartnerCenter.Offers;
    using Models.PartnerCenter.Orders;
    using Models.PartnerCenter.Subscriptions;
    using Newtonsoft.Json;
    using Services;
    using Services.PartnerCenter;

    public static class AuditRecordConverter
    {
        /// <summary>
        /// Converts an audit record to the specified type.
        /// </summary>
        /// <typeparam name="TOutput">Type of object to be returned.</typeparam>
        /// <param name="record">An audit record from Partner Center.</param>
        /// <returns>An entity that represents the modified resource.</returns>
        public static TOutput Convert<TOutput>(AuditRecord record)
        {
            return JsonConvert.DeserializeObject<TOutput>(record.ResourceNewValue);
        }

        /// <summary>
        /// Converts the audit records to a list of customer details.
        /// </summary>
        /// <param name="client">Provides the ability to interface with Partner Center.</param>
        /// <param name="records">A list of audit records from Partner Center.</param>
        /// <param name="details">A list of existing customer details.</param>
        /// <param name="additionalInfo">Additional information to be added to the converted customer details.</param>
        /// <returns>
        /// A list of customer details that incorporates the changes reflected by the audit records.
        /// </returns>
        public static async Task<List<CustomerDetail>> ConvertAsync(
            IPartnerServiceClient client,
            List<AuditRecord> records,
            List<CustomerDetail> details,
            Dictionary<string, string> additionalInfo,
            ILogger log)
        {
            Customer resource;
            CustomerDetail control;
            IEnumerable<AuditRecord> filtered;
            List<CustomerDetail> results;

            try
            {
                // Filter the audit records, so that only records for successful operations are considered.
                filtered = records
                    .Where(r => r.OperationStatus == OperationStatus.Succeeded && !string.IsNullOrEmpty(r.CustomerId))
                    .OrderBy(r => r.OperationDate);

                results = new List<CustomerDetail>(details);

                foreach (AuditRecord record in filtered)
                {
                    control = results.SingleOrDefault(r => r.Id.Equals(record.CustomerId, StringComparison.InvariantCultureIgnoreCase));

                    /*
                     * If the control variable is null and the operation type value is not equal 
                     * to AddCustomer, then that means we have a customer that accepted the reseller 
                     * relationship. Currently there is not an audit record for when a customer 
                     * accepts the reseller relationship.
                     */

                    if (control == null && record.OperationType != OperationType.AddCustomer)
                    {
                        try
                        {
                            resource = await client.Customers[record.CustomerId].GetAsync().ConfigureAwait(false);

                            results.Add(
                                ResourceConverter.Convert<Customer, CustomerDetail>(
                                    resource,
                                    additionalInfo));
                        }
                        catch (ServiceClientException ex)
                        {
                            log.LogError($"Unable to process the customer with the identifier of {record.CustomerId}", ex);
                        }
                    }
                    else if (control != null)
                    {
                        control.RemovedFromPartnerCenter = false;
                    }
                    else if (record.OperationType == OperationType.AddCustomer)
                    {
                        resource = Convert<Customer>(record);

                        results.Add(
                            ResourceConverter.Convert<Customer, CustomerDetail>(
                                resource,
                                additionalInfo));
                    }
                    else if (record.OperationType == OperationType.RemovePartnerCustomerRelationship)
                    {
                        control.RemovedFromPartnerCenter = true;
                    }
                    else if (record.OperationType == OperationType.UpdateCustomerBillingProfile)
                    {
                        control.BillingProfile = Convert<CustomerBillingProfile>(record);
                    }
                }

                return results;
            }
            finally
            {
                control = null;
                filtered = null;
                resource = null;
            }
        }

        /// <summary>
        /// Converts the audit records to a list of subscription details.
        /// </summary>
        /// <param name="client">Provides the ability to interface with Partner Center.</param>
        /// <param name="records">A list of audit records from Partner Center.</param>
        /// <param name="details">A list of existing subscription details.</param>
        /// <param name="customer">The details for the customer that owns the subscription.</param>
        /// <returns>
        /// A list of subscription details that incorporates the changes reflected by the audit records.
        /// </returns>
        public static async Task<List<SubscriptionDetail>> ConvertAsync(
            IPartnerServiceClient client,
            IEnumerable<AuditRecord> auditRecords,
            List<SubscriptionDetail> details,
            CustomerDetail customer)
        {
            IEnumerable<AuditRecord> filteredRecords;
            List<SubscriptionDetail> fromOrders;
            List<SubscriptionDetail> results;
            SubscriptionDetail control;
            Subscription resource;

            try
            {
                // Extract a list of audit records that are scope to the defined resource type and were successful.
                filteredRecords = auditRecords
                    .Where(r => (r.ResourceType == ResourceType.Subscription || r.ResourceType == ResourceType.Order)
                        && r.OperationStatus == OperationStatus.Succeeded)
                    .OrderBy(r => r.OperationDate);

                results = new List<SubscriptionDetail>(details);

                foreach (AuditRecord record in filteredRecords)
                {
                    if (record.ResourceType == ResourceType.Order)
                    {
                        fromOrders = await ConvertAsync(
                            client,
                            customer,
                            Convert<Order>(record)).ConfigureAwait(false);

                        if (fromOrders != null)
                        {
                            results.AddRange(fromOrders);
                        }
                    }
                    else if (record.ResourceType == ResourceType.Subscription)
                    {
                        resource = Convert<Subscription>(record);
                        control = results.SingleOrDefault(r => r.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                        if (control != null)
                        {
                            results.Remove(control);
                        }

                        results.Add(
                            ResourceConverter.Convert<Subscription, SubscriptionDetail>(
                                resource,
                                new Dictionary<string, string> { { "TenantId", customer.Id } }));
                    }
                }

                return results;
            }
            finally
            {
                control = null;
                filteredRecords = null;
                fromOrders = null;
                resource = null;
            }
        }


        private static async Task<List<SubscriptionDetail>> ConvertAsync(
            IPartnerServiceClient partner,
            CustomerDetail customer,
            Order order)
        {
            DateTime effectiveStartDate;
            DateTimeOffset creationDate;
            List<SubscriptionDetail> details;
            Offer offer;

            try
            {
                if (order.BillingCycle == BillingCycleType.OneTime)
                {
                    return null;
                }

                details = new List<SubscriptionDetail>();

                foreach (OrderLineItem lineItem in order.LineItems)
                {
                    creationDate = order.CreationDate.Value;

                    effectiveStartDate = new DateTime(
                            creationDate.UtcDateTime.Year,
                            creationDate.UtcDateTime.Month,
                            creationDate.UtcDateTime.Day);

                    offer = await partner.Offers
                        .ByCountry(customer.BillingProfile.DefaultAddress.Country).ById(lineItem.OfferId)
                        .GetAsync().ConfigureAwait(false);

                    details.Add(new SubscriptionDetail
                    {
                        AutoRenewEnabled = offer.IsAutoRenewable,
                        BillingCycle = order.BillingCycle,
                        BillingType = offer.Billing,
                        CommitmentEndDate = (offer.Billing == BillingType.License) ?
                            effectiveStartDate.AddYears(1) : DateTime.Parse("9999-12-14T00:00:00Z", CultureInfo.CurrentCulture),
                        CreationDate = creationDate.UtcDateTime,
                        EffectiveStartDate = effectiveStartDate,
                        FriendlyName = lineItem.FriendlyName,
                        Id = lineItem.SubscriptionId,
                        OfferId = lineItem.OfferId,
                        OfferName = offer.Name,
                        ParentSubscriptionId = lineItem.ParentSubscriptionId,
                        PartnerId = lineItem.PartnerIdOnRecord,
                        Quantity = lineItem.Quantity,
                        Status = SubscriptionStatus.Active,
                        SuspensionReasons = null,
                        TenantId = customer.Id,
                        UnitType = offer.UnitType
                    });
                }

                return details;
            }
            finally
            {
                offer = null;
            }
        }
    }
}