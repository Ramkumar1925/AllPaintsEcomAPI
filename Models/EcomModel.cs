namespace AllPaintsEcomAPI.Models
{
    public class createDtls
    {

        public List<cartItem> cartItem { get; set; }
        public List<suggestItem> suggestItem { get; set; }
        public List<compareItem> compareItem { get; set; }
    }
    public class cartItem
    {
        public string? user_code { get; set; }
        public string? materialName { get; set; }
        public string? materialCode { get; set; }
        public string? pack { get; set; }
        public decimal? price { get; set; }
        public decimal? GSTPrice { get; set; }
        public string? color { get; set; }
        public int? qty { get; set; }
        public decimal? stock { get; set; }
        public string? img { get; set; }
        public decimal? GstPer { get; set; }
        public string? paints { get; set; }
        public string? ecom { get; set; }
        public decimal? Updated_price { get; set; }
        public decimal? GSTUpdated_price { get; set; }
        public string? Offer { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class suggestItem
    {
        public string? user_code { get; set; }
        public string? materialName { get; set; }
        public string? materialCode { get; set; }
        public string? pack { get; set; }
        public decimal? price { get; set; }
        public decimal? GSTPrice { get; set; }
        public string? color { get; set; }
        public int? qty { get; set; }
        public decimal? stock { get; set; }
        public string? img { get; set; }
        public decimal? GstPer { get; set; }
        public string? paints { get; set; }
        public string? ecom { get; set; }
        public decimal? Updated_price { get; set; }
        public decimal? GSTUpdated_price { get; set; }
        public string? Offer { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class compareItem
    {
        public string? user_code { get; set; }
        public string? materialName { get; set; }
        public string? materialCode { get; set; }
        public string? pack { get; set; }
        public decimal? price { get; set; }
        public decimal? GSTPrice { get; set; }
        public string? color { get; set; }
        public int? qty { get; set; }
        public decimal? stock { get; set; }
        public string? img { get; set; }
        public decimal? GstPer { get; set; }
        public string? paints { get; set; }
        public string? ecom { get; set; }
        public decimal? Updated_price { get; set; }
        public decimal? GSTUpdated_price { get; set; }
        public string? Offer { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class updateCart
    {
        public string? user_code { get; set; }
        public string? materialCode { get; set; }
        public string? ecomCode { get; set; }
        public string? packSize { get; set; }
        public string? qty { get; set; }
        public string? RefrenceCode { get; set; }
    }

    public class deleteCart
    {
        public string? user_code { get; set; }
        public string? materialCode { get; set; }
        public string? unique_cart { get; set; }
        public List<string>? unique_compare { get; set; }
        public string? unique_suggest { get; set; }
    }


    public class ResponseDataModel
    {
        public List<cartItem> cartItems { get; set; }
        public List<suggestItem> suggestItem { get; set; }
        public List<compareItem> compareItem { get; set; }
    }

    public class ApiGetResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int count { get; set; }
        public ResponseDataModel data { get; set; }

        public string Error { get; set; }
    }

    //---------------Payment proccess----------------------
    public class metaInfo
    {
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
    }

    public class merchantUrls
    {
        public string redirectUrl { get; set; }
    }

    public class paymentFlow
    {
        public string type { get; set; }
        public string message { get; set; }
        public merchantUrls merchantUrls { get; set; }
    }

    public class PaymentRequest
    {
        public string merchantOrderId { get; set; }
        public int amount { get; set; }
        public int expireAfter { get; set; }
        public metaInfo metaInfo { get; set; }
        public paymentFlow paymentFlow { get; set; }
    }

    public class PhonePeResponse
    {
        public int StatusCode { get; set; }
        public string Msg { get; set; }
        public List<string> Error { get; set; }
        public string Data { get; set; } // JWT Token string
    }

    public class PhonePeReturn
    {
        public string? orderId { get; set; }
        public string? state { get; set; }
        public long? expireAt { get; set; }
        public string? redirectUrl { get; set; }
    }

    public class metaInfo1
    {
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
    }

    public class rail
    {
        public string type { get; set; }
        public string transactionId { get; set; }
        public string authorizationCode { get; set; }
        public string serviceTransactionId { get; set; }
    }

    public class instrument
    {
        public string type { get; set; }
        public string bankTransactionId { get; set; }
        public string bankId { get; set; }
        public string arn { get; set; }
        public string brn { get; set; }
    }

    public class splitInstrument
    {
        public int amount { get; set; }
        public rail rail { get; set; }
        public instrument instrument { get; set; }
    }

    public class paymentDetail
    {
        public string paymentMode { get; set; }
        public string transactionId { get; set; }
        public long timestamp { get; set; }
        public int amount { get; set; }
        public string state { get; set; }
        public List<splitInstrument> splitInstruments { get; set; }
    }

    public class paymentStatusResponse
    {
        public string orderId { get; set; }
        public string state { get; set; }
        public int amount { get; set; }
        public long expireAt { get; set; }
        public metaInfo1 metaInfo { get; set; }
        public List<paymentDetail> paymentDetails { get; set; }
    }

    public class paymentResponse
    {
        public int statusCode { get; set; }
        public string msg { get; set; }
        public string payUrl { get; set; }
        public string Status { get; set; }
    }

    //-------------------------------------

}
