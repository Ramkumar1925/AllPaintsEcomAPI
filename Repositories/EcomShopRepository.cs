using AllPaintsEcomAPI.DTO;

namespace AllPaintsEcomAPI.Repositories
{
    public interface EcomShopRepository
    {
        Task<string> cart(string user_code);
        Task<string> cart(updateCart prm);
        Task<string> cart(string user_code, string ecomCode, string packSize, string RefrenceCode);
        //Task<string> cart(createDtls prm);
        Task<string> cart(createDtls prm)
        {
            throw new NotImplementedException();
        }
        Task<string> GetALPNProducts_DoctypeNew(dynamic prm)
        {
            throw new NotImplementedException();
        }
        Task<string> GetAllpaintprofileprocess(dynamic prm)
        {
            throw new NotImplementedException();
        }
        Task<string> getAllpaintsrgbcolor(dynamic prm)
        {
            throw new NotImplementedException();
        }
        Task<string> FetchCustomerDtls(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> AllPaintsOrderData(dynamic prm) { 
            throw new NotImplementedException();
        }

        Task<string> GatwayPaymentProcess(dynamic prm)
        {
            throw new NotImplementedException();
        }
        Task<string> GatwayPaymentDtls(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> PainterGenerateOtp(dynamic prm)
        { 
            throw new NotImplementedException();
        }

        Task<string> PainterOTPVerify(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> customerGenOTP(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> GetFranchise_Customer_Creation(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> createCustomerInfo(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> OtpVerifyed(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> SaveCustomerCreation(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> SavePainterCreation(dynamic prm)
        {
            throw new NotImplementedException();
        }

        Task<string> GetFranchisePendingOrder(dynamic prm)
        {
            throw new NotImplementedException();
        }
        Task<string> EcommerceWhatsappPdf(dynamic prm)
        {
            throw new NotImplementedException();
        }


    }
}
