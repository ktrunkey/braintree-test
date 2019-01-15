using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Braintree;

namespace BraintreeTest.Controllers {

  public class PaymentsController : Controller {

    public IBraintreeConfiguration BraintreeConfiguration = new BraintreeConfiguration();

    public static readonly TransactionStatus[] transactionSuccessStatuses = {
      TransactionStatus.AUTHORIZED,
      TransactionStatus.AUTHORIZING,
      TransactionStatus.SETTLED,
      TransactionStatus.SETTLING,
      TransactionStatus.SETTLEMENT_CONFIRMED,
      TransactionStatus.SETTLEMENT_PENDING,
      TransactionStatus.SUBMITTED_FOR_SETTLEMENT
    };


    // GET: Payments
    public ActionResult Index() {

      var gateway = BraintreeConfiguration.GetGateway();
      var clientToken = gateway.ClientToken.Generate();
      ViewBag.ClientToken = clientToken;

      return View();
    }

    public ActionResult Create() {
      var gateway = BraintreeConfiguration.GetGateway();
      Decimal amount;

      try {
        amount = Convert.ToDecimal(Request["amount"]);
      }
      catch (FormatException e) {
        TempData["Flash"] = "Error: 81503: Amount is an invalid format.";
        return RedirectToAction("Index");
      }

      var nonce = Request["payment_method_nonce"];
      var request = new TransactionRequest {
        Amount = amount,
        PaymentMethodNonce = nonce,
        CustomFields = new Dictionary<string, string>
        {
            { "company", Request["company"] },
            { "invoice", Request["invoice"] }
        },
        Options = new TransactionOptionsRequest {
          SubmitForSettlement = true
        }
      };

      Result<Transaction> result = gateway.Transaction.Sale(request);
      if (result.IsSuccess()) {
        Transaction transaction = result.Target;
        return RedirectToAction("Confirmation", new { id = transaction.Id });
      }
      else if (result.Transaction != null) {
        return RedirectToAction("Confirmation", new { id = result.Transaction.Id } );
      }
      else {
        string errorMessages = "";
        foreach (ValidationError error in result.Errors.DeepAll()) {
          errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
        }
        TempData["Flash"] = errorMessages;
        return RedirectToAction("Index");
      }

    }

    public ActionResult Confirmation(String id) {

      var gateway = BraintreeConfiguration.GetGateway();
      Transaction transaction = gateway.Transaction.Find(id);

      if (transactionSuccessStatuses.Contains(transaction.Status)) {
        TempData["header"] = "Sweet Success!";
        TempData["icon"] = "success";
        TempData["message"] = "Your test transaction has been successfully processed. See the Braintree API response and try again.";
      }
      else {
        TempData["header"] = "Transaction Failed";
        TempData["icon"] = "fail";
        TempData["message"] = "Your test transaction has a status of " + transaction.Status + ". See the Braintree API response and try again.";
      };

      ViewBag.Transaction = transaction;
      return View();

    }

  }
}