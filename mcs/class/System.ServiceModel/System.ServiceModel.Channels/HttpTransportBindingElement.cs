//
// HttpTransportBindingElement.cs
//
// Author:
//	Atsushi Enomoto <atsushi@ximian.com>
//
// Copyright (C) 2005-2010 Novell, Inc.  http://www.novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
#if NET_4_0
using System.Security.Authentication.ExtendedProtection;
#endif
using System.ServiceModel.Channels;
#if !NET_2_1
using System.ServiceModel.Channels.Http;
#endif
using System.ServiceModel.Description;
#if !MOBILE
using WS = System.Web.Services.Description;
#endif
using System.Xml;

namespace System.ServiceModel.Channels
{
	public class HttpTransportBindingElement : TransportBindingElement,
		IPolicyExportExtension, IWsdlExportExtension
	{
		bool allow_cookies, bypass_proxy_on_local,
			unsafe_ntlm_auth;
		bool use_default_proxy = true, keep_alive_enabled = true;
		int max_buffer_size = 0x10000;
		HostNameComparisonMode host_cmp_mode;
		Uri proxy_address;
		string realm = String.Empty;
		TransferMode transfer_mode;
		IDefaultCommunicationTimeouts timeouts;
		AuthenticationSchemes auth_scheme =
			AuthenticationSchemes.Anonymous;
		AuthenticationSchemes proxy_auth_scheme =
			AuthenticationSchemes.Anonymous;
		// If you add fields, do not forget them in copy constructor.

		public HttpTransportBindingElement ()
		{
		}

		protected HttpTransportBindingElement (
			HttpTransportBindingElement other)
			: base (other)
		{
			allow_cookies = other.allow_cookies;
			bypass_proxy_on_local = other.bypass_proxy_on_local;
			unsafe_ntlm_auth = other.unsafe_ntlm_auth;
			use_default_proxy = other.use_default_proxy;
			keep_alive_enabled = other.keep_alive_enabled;
			max_buffer_size = other.max_buffer_size;
			host_cmp_mode = other.host_cmp_mode;
			proxy_address = other.proxy_address;
			realm = other.realm;
			transfer_mode = other.transfer_mode;
			// FIXME: it does not look safe
			timeouts = other.timeouts;
			auth_scheme = other.auth_scheme;
			proxy_auth_scheme = other.proxy_auth_scheme;

#if NET_4_0
			DecompressionEnabled = other.DecompressionEnabled;
			LegacyExtendedProtectionPolicy = other.LegacyExtendedProtectionPolicy;
			ExtendedProtectionPolicy = other.ExtendedProtectionPolicy;
#endif
		}

#if NET_4_0
		[DefaultValue (AuthenticationSchemes.Anonymous)]
#endif
		public AuthenticationSchemes AuthenticationScheme {
			get { return auth_scheme; }
			set { auth_scheme = value; }
		}

#if NET_4_0
		[DefaultValue (AuthenticationSchemes.Anonymous)]
#endif
		public AuthenticationSchemes ProxyAuthenticationScheme {
			get { return proxy_auth_scheme; }
			set { proxy_auth_scheme = value; }
		}

#if NET_4_0
		[DefaultValue (false)]
#endif
		public bool AllowCookies {
			get { return allow_cookies; }
			set { allow_cookies = value; }
		}

#if NET_4_0
		[DefaultValue (false)]
#endif
		public bool BypassProxyOnLocal {
			get { return bypass_proxy_on_local; }
			set { bypass_proxy_on_local = value; }
		}

#if NET_4_0
		[DefaultValue (false)]
		[MonoTODO]
		public bool DecompressionEnabled { get; set; }
#endif

#if NET_4_0
		[DefaultValue (HostNameComparisonMode.StrongWildcard)]
#endif
		public HostNameComparisonMode HostNameComparisonMode {
			get { return host_cmp_mode; }
			set { host_cmp_mode = value; }
		}

#if NET_4_0
		[DefaultValue (true)]
#endif
		public bool KeepAliveEnabled {
			get { return keep_alive_enabled; }
			set { keep_alive_enabled = value; }
		}

#if NET_4_0
		[DefaultValue (0x10000)]
#endif
		public int MaxBufferSize {
			get { return max_buffer_size; }
			set { max_buffer_size = value; }
		}

#if NET_4_0
		[DefaultValue (null)]
		[TypeConverter (typeof (UriTypeConverter))]
#endif
		public Uri ProxyAddress {
			get { return proxy_address; }
			set { proxy_address = value; }
		}

#if NET_4_0
		[DefaultValue ("")]
#endif
		public string Realm {
			get { return realm; }
			set { realm = value; }
		}

		public override string Scheme {
			get { return Uri.UriSchemeHttp; }
		}

#if NET_4_0
		[DefaultValue (TransferMode.Buffered)]
#endif
		public TransferMode TransferMode {
			get { return transfer_mode; }
			set { transfer_mode = value; }
		}

#if NET_4_0
		[DefaultValue (false)]
#endif
		public bool UnsafeConnectionNtlmAuthentication {
			get { return unsafe_ntlm_auth; }
			set { unsafe_ntlm_auth = value; }
		}

#if NET_4_0
		[DefaultValue (true)]
#endif
		public bool UseDefaultWebProxy {
			get { return use_default_proxy; }
			set { use_default_proxy = value; }
		}

#if NET_4_0
		[Obsolete ("Use ExtendedProtectionPolicy")]
		[MonoTODO]
		public object LegacyExtendedProtectionPolicy { get; set; }

		[MonoTODO]
		public ExtendedProtectionPolicy ExtendedProtectionPolicy { get; set; }
#endif

		public override bool CanBuildChannelFactory<TChannel> (
			BindingContext context)
		{
			return typeof (TChannel) == typeof (IRequestChannel);
		}

#if !NET_2_1
		public override bool CanBuildChannelListener<TChannel> (
			BindingContext context)
		{
			return typeof (TChannel) == typeof (IReplyChannel);
		}
#endif

		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel> (
			BindingContext context)
		{
			// remaining contexts are ignored ... e.g. such binding
			// element that always causes an error is ignored.
			return new HttpChannelFactory<TChannel> (this, context);
		}

#if !NET_2_1
		internal static object ListenerBuildLock = new object ();

		public override IChannelListener<TChannel> BuildChannelListener<TChannel> (
			BindingContext context)
		{
			// remaining contexts are ignored ... e.g. such binding
			// element that always causes an error is ignored.
			return new HttpChannelListener<TChannel> (this, context);
		}
#endif

		public override BindingElement Clone ()
		{
			return new HttpTransportBindingElement (this);
		}

		public override T GetProperty<T> (BindingContext context)
		{
			// http://blogs.msdn.com/drnick/archive/2007/04/10/interfaces-for-getproperty-part-1.aspx
			if (typeof (T) == typeof (ISecurityCapabilities))
				return (T) (object) new HttpBindingProperties (this);
			if (typeof (T) == typeof (IBindingDeliveryCapabilities))
				return (T) (object) new HttpBindingProperties (this);
			if (typeof (T) == typeof (TransferMode))
				return (T) (object) TransferMode;
			return base.GetProperty<T> (context);
		}
		
#if NET_4_5
		public WebSocketTransportSettings WebSocketSettings {
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}
#endif

#if !NET_2_1
		void IPolicyExportExtension.ExportPolicy (
			MetadataExporter exporter,
			PolicyConversionContext context)
		{
			if (exporter == null)
				throw new ArgumentNullException ("exporter");
			if (context == null)
				throw new ArgumentNullException ("context");

			PolicyAssertionCollection assertions = context.GetBindingAssertions ();
			XmlDocument doc = new XmlDocument ();

			ExportAddressingPolicy (context);

			switch (auth_scheme) {
			case AuthenticationSchemes.Basic:
			case AuthenticationSchemes.Digest:
			case AuthenticationSchemes.Negotiate:
			case AuthenticationSchemes.Ntlm:
				assertions.Add (doc.CreateElement ("http", 
						auth_scheme.ToString () + "Authentication", 
						"http://schemas.microsoft.com/ws/06/2004/policy/http"));
				break;
			}

			var transportProvider = this as ITransportTokenAssertionProvider;
			if (transportProvider != null) {
				var token = transportProvider.GetTransportTokenAssertion ();
				assertions.Add (CreateTransportBinding (token));
			}
		}

		XmlElement CreateTransportBinding (XmlElement transportToken)
		{
			var doc = new XmlDocument ();
			var transportBinding = doc.CreateElement (
				"sp", "TransportBinding", PolicyImportHelper.SecurityPolicyNS);

			var token = doc.CreateElement (
				"sp", "TransportToken", PolicyImportHelper.SecurityPolicyNS);
			PolicyImportHelper.AddWrappedPolicyElement (token, transportToken);

			var algorithmSuite = doc.CreateElement (
				"sp", "AlgorithmSuite", PolicyImportHelper.SecurityPolicyNS);
			var basic256 = doc.CreateElement (
				"sp", "Basic256", PolicyImportHelper.SecurityPolicyNS);
			PolicyImportHelper.AddWrappedPolicyElement (algorithmSuite, basic256);

			var layout = doc.CreateElement (
				"sp", "Layout", PolicyImportHelper.SecurityPolicyNS);
			var strict = doc.CreateElement (
				"sp", "Strict", PolicyImportHelper.SecurityPolicyNS);
			PolicyImportHelper.AddWrappedPolicyElement (layout, strict);

			PolicyImportHelper.AddWrappedPolicyElements (
				transportBinding, token, algorithmSuite, layout);

			return transportBinding;
		}

		[MonoTODO]
		void IWsdlExportExtension.ExportContract (WsdlExporter exporter,
			WsdlContractConversionContext context)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		void IWsdlExportExtension.ExportEndpoint (WsdlExporter exporter,
			WsdlEndpointConversionContext context)
		{
			var soap_binding = new WS.SoapBinding ();
			soap_binding.Transport = WS.SoapBinding.HttpTransport;
			soap_binding.Style = WS.SoapBindingStyle.Document;
			context.WsdlBinding.Extensions.Add (soap_binding);

			var soap_address = new WS.SoapAddressBinding ();
			soap_address.Location = context.Endpoint.Address.Uri.AbsoluteUri;
			
			context.WsdlPort.Extensions.Add (soap_address);
		}
#endif
	}

	class HttpBindingProperties : ISecurityCapabilities, IBindingDeliveryCapabilities
	{
		HttpTransportBindingElement source;

		public HttpBindingProperties (HttpTransportBindingElement source)
		{
			this.source = source;
		}

		public bool AssuresOrderedDelivery {
			get { return false; }
		}

		public bool QueuedDelivery {
			get { return false; }
		}

		public virtual ProtectionLevel SupportedRequestProtectionLevel {
			get { return ProtectionLevel.None; }
		}

		public virtual ProtectionLevel SupportedResponseProtectionLevel {
			get { return ProtectionLevel.None; }
		}

		public virtual bool SupportsClientAuthentication {
			get { return source.AuthenticationScheme != AuthenticationSchemes.Anonymous; }
		}

		public virtual bool SupportsServerAuthentication {
			get {
				switch (source.AuthenticationScheme) {
				case AuthenticationSchemes.Negotiate:
					return true;
				default:
					return false;
				}
			}
		}

		public virtual bool SupportsClientWindowsIdentity {
			get {
				switch (source.AuthenticationScheme) {
				case AuthenticationSchemes.Basic:
				case AuthenticationSchemes.Digest: // hmm... why? but they return true on .NET
				case AuthenticationSchemes.Negotiate:
				case AuthenticationSchemes.Ntlm:
					return true;
				default:
					return false;
				}
			}
		}
	}
}
