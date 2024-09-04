using Payment.Api.Domain;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Payment.Api.Events;
using Payment.Api.Messaging;

namespace Payment.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IEventBus _eventBus;

    public PaymentService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<bool> Pay(PaymentRequestModel requestModel)
    {
        var (request, options) = PreparePaymentRequest(requestModel);
        var payment = Iyzipay.Model.Payment.Create(request, options);

        if (payment.Status == Status.SUCCESS.ToString())
        {
            var id = Guid.NewGuid();
            await _eventBus.PublishAsync(new PaymentReceivedEvent()
            {
                Id = id,
                AccountId = requestModel.AccountId,
                Price = Convert.ToDecimal(request.Price),
                Date = DateTime.UtcNow
            },"payment-events",id.ToString());
            
            return true;
        }

        return false;
    }

    private (CreatePaymentRequest paymentRequest, Options options) PreparePaymentRequest(
        PaymentRequestModel requestModel)
    {
        var options = new Options()
        {
            ApiKey = "sandbox-CVRAMKiFtlncoSupnXQl7nlUY75LaxOP",
            BaseUrl = "https://sandbox-api.iyzipay.com",
            SecretKey = "sandbox-NzPtHg3EvposHII1OXBsEVqHnlZ0Wp0T"
        };

        CreatePaymentRequest request = new CreatePaymentRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = "123456789";
        request.Price = requestModel.Price.ToString(); // Ürün fiyatı
        request.PaidPrice = requestModel.Price.ToString(); // Ödenecek toplam fiyat
        request.Currency = Currency.TRY.ToString();
        request.Installment = 1;
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
        request.PaymentChannel = PaymentChannel.WEB.ToString();
        request.BasketId = "B67832";

        PaymentCard paymentCard = new PaymentCard();
        paymentCard.CardHolderName = "John Doe";
        paymentCard.CardNumber = "5528790000000008"; // Kart numarası
        paymentCard.ExpireMonth = "12"; // Son kullanma ayı
        paymentCard.ExpireYear = "2030"; // Son kullanma yılı
        paymentCard.Cvc = "123"; // CVC
        paymentCard.RegisterCard = 0; // Kart kaydetme seçeneği

        request.PaymentCard = paymentCard;

        Buyer buyer = new Buyer();
        buyer.Id = "BY789";
        buyer.Name = "John";
        buyer.Surname = "Doe";
        buyer.IdentityNumber = "74300864791";
        buyer.Email = "email@email.com";
        buyer.GsmNumber = "+905350000000";
        buyer.RegistrationDate = "2013-04-21 15:12:09";
        buyer.LastLoginDate = "2015-10-05 12:43:35";
        buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        buyer.City = "Istanbul";
        buyer.Country = "Turkey";
        buyer.ZipCode = "34732";
        buyer.Ip = "85.34.78.112";

        request.Buyer = buyer;

        Address shippingAddress = new Address();
        shippingAddress.ContactName = "Jane Doe";
        shippingAddress.City = "Istanbul";
        shippingAddress.Country = "Turkey";
        shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        shippingAddress.ZipCode = "34742";
        request.ShippingAddress = shippingAddress;

        Address billingAddress = new Address();
        billingAddress.ContactName = "Jane Doe";
        billingAddress.City = "Istanbul";
        billingAddress.Country = "Turkey";
        billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        billingAddress.ZipCode = "34742";
        request.BillingAddress = billingAddress;

        List<BasketItem> basketItems = new List<BasketItem>();

        BasketItem firstBasketItem = new BasketItem();
        firstBasketItem.Id = "BI101";
        firstBasketItem.Name = "Binocular";
        firstBasketItem.Category1 = "Collectibles";
        firstBasketItem.Category2 = "Accessories";
        firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
        firstBasketItem.Price = requestModel.Price.ToString(); // Fiyat
        basketItems.Add(firstBasketItem);

        

        request.BasketItems = basketItems;
        return (request, options);
    }
}