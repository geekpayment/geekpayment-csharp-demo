using Xunit;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using GeekPaymentSample.Payment;
using GeekPaymentSample.Geek.Payment;
using GeekPaymentSample.Geek;

namespace GeekPaymentSample.Tests
{
    public class GeekOrderCloseTest
    {
        [Fact]
        public void Test()
        {
            //创建订单
            OrderInfo orderInfo = CreateOrder();
            
            GeekOrderClose close = GetClose();

            OrderInfo existOrderInfo = close.Close(orderInfo.MchOrderId);

            Assert.Equal(orderInfo.MchSerialNo, existOrderInfo.MchSerialNo);
            Assert.Equal(orderInfo.OrderId, existOrderInfo.OrderId);
            Assert.Equal(orderInfo.MchOrderId, existOrderInfo.MchOrderId);
            Assert.Equal(orderInfo.Price, existOrderInfo.Price);
            Assert.Equal(orderInfo.MchDiscount, existOrderInfo.MchDiscount);
            Assert.Equal(orderInfo.PlatformDiscount, existOrderInfo.PlatformDiscount);
            Assert.Equal(orderInfo.TotalAmount, existOrderInfo.TotalAmount);
            Assert.Equal(orderInfo.PayAmount, existOrderInfo.PayAmount);
            Assert.Equal(orderInfo.ToSettleAmount, existOrderInfo.ToSettleAmount);
            Assert.Equal(orderInfo.Channel, existOrderInfo.Channel);
            Assert.Equal(orderInfo.CreateTime, existOrderInfo.CreateTime);
            Assert.NotEmpty(existOrderInfo.PayTime);
            Assert.NotEmpty(existOrderInfo.OrderStatus);

            Console.WriteLine(JsonConvert.SerializeObject(existOrderInfo));
        }

        private GeekOrderClose GetClose()
        {
            
            GeekSign geekSign = new GeekSign(AppProperties.GeekPublicKey, AppProperties.PrivateKey);

            HttpClient httpClient = new HttpClient();
            GeekEndPoint geekEndPoint = new GeekEndPoint(httpClient, geekSign);

            GeekUriComponents uriComponents = new GeekUriComponents(GeekPaymentProperties.Scheme, GeekPaymentProperties.Host, geekSign);

            HttpResponseParser<OrderInfo> responseParser = new OrderInfoResponseParser();

            return new GeekOrderClose(uriComponents, geekEndPoint, responseParser, AppProperties.AppID);
        }

        private OrderInfo CreateOrder()
        {
            GeekChannelOrderFactory geekChannelOrderFactory = new GeekChannelOrderFactory();
            OrderCreateInfo orderCreateInfo = new OrderCreateInfo();
            orderCreateInfo.MchOrderId = string.Format("ORDER{0}", DateTime.Now.Ticks);
            orderCreateInfo.Title = "付款码下单订单";
            orderCreateInfo.Price = 12300;
            orderCreateInfo.Expire = "10m";
            
            string authCode = "284896288107264920";
            
            string deviceId = "111";

            ChannelOrder channelOrder = geekChannelOrderFactory.RetailOrder();
            return channelOrder.Create(orderCreateInfo, authCode, deviceId);
        }
    }
}