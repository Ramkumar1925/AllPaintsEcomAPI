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

}
