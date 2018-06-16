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
        /// HTTP status code associated with the exception.
        /// </summary>
        [NonSerialized]
        private readonly HttpStatusCode httpStatusCode;

        /// <summary>
        /// Identifier of the customer associated with the exception.
        /// </summary>
        [NonSerialized]
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException" /> class.
        /// </summary>
        public ServiceClientException()
        {
            httpStatusCode = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceClientException(string message) : base(message)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ServiceClientException(string message, Exception innerException) : base(message, innerException)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatusCode">The HTTP status code that was encountered with the error.</param>
        public ServiceClientException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="httpStatusCode">The HTTP status code that was encountered with the error.</param>
        public ServiceClientException(string message, string customerId, HttpStatusCode httpStatusCode) : base(message)
        {
            this.customerId = customerId;
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientException"/> class.
        /// </summary>
        /// <param name="info">The serialization infomration that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The context that contains contextual information about the source or destination.</param>
        protected ServiceClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            customerId = serializationInfo.GetString("customerId");
            httpStatusCode = (HttpStatusCode)serializationInfo.GetValue("httpStatusCode", typeof(HttpStatusCode));
        }

        /// <summary>
        /// When overridden in a derived class, sets the serialization information with information about the exception.
        /// </summary>
        /// <param name="info">The serialization information that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The context that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("CustomerId", customerId);
            info.AddValue("HttpStatusCode", (int)httpStatusCode);

            base.GetObjectData(info, context);
        }
    }
}