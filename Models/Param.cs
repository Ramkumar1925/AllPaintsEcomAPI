namespace AllPaintsEcomAPI.Models
{
    public class Param
    {
        public string? filtervalue1 { get; set; }
        public string? filtervalue2 { get; set; }
        public string? filtervalue3 { get; set; }
        public string? filtervalue4 { get; set; }
        public string? filtervalue5 { get; set; }
        public string? filtervalue6 { get; set; }
        public string? filtervalue7 { get; set; }
        public string? filtervalue8 { get; set; }
        public string? filtervalue9 { get; set; }
        public string? filtervalue10 { get; set; }
        public string? filtervalue11 { get; set; }
        public string? filtervalue12 { get; set; }
        public string? filtervalue13 { get; set; }
        public string? filtervalue14 { get; set; }
        public string? filtervalue15 { get; set; }
    }

    public class APIResponse
    {
        public int status { get; set; }
        public string statusText { get; set; }
        public string Error { get; set; }
        public object[] body { get; set; }
    }

    public class ApiResponse1
    {
        public int ndocno { get; set; }
        public string Message { get; set; }
    }

    public class EmployeenewDTO
    {
        public string employeecode { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobileno { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string Roll_id { get; set; }
        public string Roll_name { get; set; }
        public string ReportManagerid { get; set; }
        public string ReportmanagerName { get; set; }
        public string ReportMgrPositioncode { get; set; }
        public string ReportMgrPositiondesc { get; set; }
        public string cdeptcode { get; set; }
        public string cdeptdesc { get; set; }

        public string cfincode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        //cfincode  StartDate   EndDate

    }


}
