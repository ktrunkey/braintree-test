﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Braintree;

namespace BraintreeTest {

  public class BraintreeConfiguration : IBraintreeConfiguration {

    public string Environment { get; set; }
    public string MerchantId { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    private IBraintreeGateway BraintreeGateway { get; set; }

    public IBraintreeGateway CreateGateway() {

      Environment = System.Environment.GetEnvironmentVariable("BraintreeEnvironment");
      MerchantId = System.Environment.GetEnvironmentVariable("BraintreeMerchantId");
      PublicKey = System.Environment.GetEnvironmentVariable("BraintreePublicKey");
      PrivateKey = System.Environment.GetEnvironmentVariable("BraintreePrivateKey");

      if (MerchantId == null || PublicKey == null || PrivateKey == null) {
        Environment = GetConfigurationSetting("BraintreeEnvironment");
        MerchantId = GetConfigurationSetting("BraintreeMerchantId");
        PublicKey = GetConfigurationSetting("BraintreePublicKey");
        PrivateKey = GetConfigurationSetting("BraintreePrivateKey");
      }

      return new BraintreeGateway(Braintree.Environment.SANDBOX, MerchantId, PublicKey, PrivateKey);
    }

    public IBraintreeGateway GetGateway() {
      if (BraintreeGateway == null) {
        BraintreeGateway = CreateGateway();
      }

      return BraintreeGateway;
    }

    public string GetConfigurationSetting(string setting) {
      return ConfigurationManager.AppSettings[setting];
    }

  }

}