// -----------------------------------------------------------------------
// <copyright file="ServiceClientException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class ServiceClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException" /> class.
        /// </summary>
        public ServiceClientException()
        {
            HttpStatusCode = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceClientException(string message) : base(message)
        {
            HttpStatusCode = HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ServiceClientException(string message, Exception innerException) : base(message, innerException)
        {
            HttpStatusCode = HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatusCode">The HTTP status code that was encountered with the error.</param>
        public ServiceClientException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="httpStatusCode">The HTTP status code that was encountered with the error.</param>
        public ServiceClientException(string message, string customerId, HttpStatusCode httpStatusCode) : base(message)
        {
            CustomerId = customerId;
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ServiceClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            CustomerId = serializationInfo.GetString("CustomerId");
            HttpStatusCode = (HttpStatusCode)serializationInfo.GetValue("HttpStatusCode", typeof(HttpStatusCode));
        }

        /// <summary>
        /// Gets the identifier of the customer for the exception.
        /// </summary>
        public string CustomerId { get; private set; }

        /// <summary>
        /// Gets the HTTP status code for the exception.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; private set; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("CustomerId", CustomerId);
            info.AddValue("HttpStatusCode", (int)HttpStatusCode);

            base.GetObjectData(info, context);
        }
    }
}