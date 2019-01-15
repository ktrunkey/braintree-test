using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Braintree;

namespace BraintreeTest {
  public interface IBraintreeConfiguration {

    IBraintreeGateway CreateGateway();
    string GetConfigurationSetting(string setting);
    IBraintreeGateway GetGateway();

  }

}