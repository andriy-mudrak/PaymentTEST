﻿using System;
using System.Threading.Tasks;
using BLL.Helpers;
using BLL.Helpers.Mapping;
using BLL.Models;
using BLL.Tests.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Stripe;

namespace BLL.Tests.HelpersTests.MappingTests
{
    [TestClass]
    public class MappingPaymentFailed_Tests
    {
        [TestMethod]
        public void Map_ValidModels_Success()
        {
            //Arrange
            var response = new
            {
                Failed = true,
            };

            var map = new MappingPaymentFailed<string>();

            var transaction = new TransactionModel
            {
                Amount = TestConstants.payment.Amount,
                Status = PaymentServiceConstants.PaymentMappingType.Failed.ToString(),
                ExternalId = null,
                Instrument = PaymentServiceConstants.PaymentMappingType.Failed.ToString(),
                OrderId = TestConstants.payment.OrderId,
                UserId = TestConstants.payment.UserId,
                VendorId = TestConstants.payment.VendorId,
                Metadata = JsonConvert.SerializeObject(TestConstants.payment),
                Response = response.ToString(),
                TransactionTime = It.IsAny<DateTime>(),
                TransactionType = It.IsAny<PaymentServiceConstants.PaymentType>().ToString(),
            };

            //Act
            var actual = map.Map(
                It.IsAny<PaymentServiceConstants.PaymentType>(),
                TestConstants.payment,
                response,
                It.IsAny<DateTime>());

            var expected = transaction;

            //Assert
            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.ExternalId, actual.ExternalId);
            Assert.AreEqual(expected.Instrument, actual.Instrument);
            Assert.AreEqual(expected.Metadata, actual.Metadata);
            Assert.AreEqual(expected.OrderId, actual.OrderId);
            Assert.AreEqual(expected.Response, actual.Response);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.TransactionTime, actual.TransactionTime);
            Assert.AreEqual(expected.TransactionType, actual.TransactionType);
            Assert.AreEqual(expected.VendorId, actual.VendorId);
        }
    }
}