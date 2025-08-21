using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using AllPaintsEcomAPI.DTO;
using AllPaintsEcomAPI.Helpers;
using AllPaintsEcomAPI.Models;
using AllPaintsEcomAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;


namespace AllPaintsEcomAPI.Services
{
    public class EcomShopService : EcomShopRepository


    {
        private readonly IConfiguration Configuration;

        private static string sapPassword = "Sheenlac@123";
        //private static string QAPassword = "Mapol@123$";
        private static string Username = string.Empty;
        private static string Password = string.Empty;
        //private static string baseAddress = "http://13.233.6.115/api/v2/auth";
        private static string baseAddress = "http://13.234.246.143/api/v2/auth";
        private static string QAPassword = "Mapol@123$";

        public EcomShopService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
 
        // Get all data
        public async Task<string> cart(string user_code)
        {
            DataSet ds1 = new DataSet();
            string query = "SELECT * FROM tbl_allpaints_user_cart WHERE TRY_CAST(user_code as int) = @user_code";    
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@user_code", user_code);

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds1);
                    con.Close();
                }
            }
            string op = JsonConvert.SerializeObject(ds1.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var model = JsonConvert.DeserializeObject<List<DTO.cartItem>>(op);


            DataSet ds2 = new DataSet();
            string query1 = "SELECT * FROM tbl_allpaints_suggested_products WHERE TRY_CAST(user_code as int) = " + "'" + user_code + "'";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {
                using (SqlCommand cmd = new SqlCommand(query1))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@user_code", user_code);

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds2);
                    con.Close();
                }
            }
            string op1 = JsonConvert.SerializeObject(ds2.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var model1 = JsonConvert.DeserializeObject<List<DTO.suggestItem>>(op1);

            DataSet ds3 = new DataSet();
            string query2 = "SELECT * FROM tbl_allpaints_compare_products WHERE TRY_CAST(user_code as int) = " + "'" + user_code + "'";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {
                using (SqlCommand cmd = new SqlCommand(query2))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@user_code", user_code);

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds3);
                    con.Close();
                }
            }
            string op2 = JsonConvert.SerializeObject(ds3.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var model2 = JsonConvert.DeserializeObject<List<DTO.compareItem>>(op2);

            if((model.Count>0) && (model1.Count>0) && (model2.Count > 0))
            {
                var response1 = new DTO.ApiGetResponse
                {
                    Status = 1,
                    Message = "Success",
                    data = new DTO.ResponseDataModel
                    {
                        cartItems = model,
                        suggestItem = model1,
                        compareItem = model2
                    }
                };
                string json = JsonConvert.SerializeObject(response1);
                var encryptCartDtls1 = AesEncryption.Encrypt(json);
                return encryptCartDtls1;
            }
            else
            {
                var response1 = new DTO.ApiGetResponse
                {
                    Status = 0,
                    Message = "Your cart is currently empty.",
                };
                string json = JsonConvert.SerializeObject(response1);
                var encryptCartDtls1 = AesEncryption.Encrypt(json);
                return encryptCartDtls1;
            }       
        }


        public async Task<string> cart(Models.updateCart prm)
        {
            int decryptedQty = int.Parse(prm.qty);
            DataSet ds1 = new DataSet();
            string query = @"
            UPDATE a 
            SET a.qty = @qty 
            FROM tbl_allpaints_user_cart a 
            WHERE TRY_CAST(user_code AS int) = @user_code 
            AND materialCode = @materialCode 
            AND RefrenceCode = @RefrenceCode";
           // string query = "update a set a.qty =" + prm.qty + " from tbl_allpaints_user_cart a where TRY_CAST(user_code as int) = " + "'" + prm.user_code + "' and materialCode = " + "'" + prm.materialCode + "' and RefrenceCode = " + "'" + prm.RefrenceCode + "'";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@user_code", prm.user_code);
                    cmd.Parameters.AddWithValue("@materialCode", prm.materialCode);
                    cmd.Parameters.AddWithValue("@RefrenceCode", prm.RefrenceCode);
                    cmd.Parameters.AddWithValue("@qty", decryptedQty);

                    con.Open();
                    int iii = cmd.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con.Close();
                }
            }

            string query1 = @"
                UPDATE a 
                SET a.qty = @qty 
                FROM tbl_allpaints_compare_products a 
                WHERE TRY_CAST(user_code AS int) = @user_code 
                AND ecom = @ecom
                AND pack = @pack
                AND RefrenceCode = @RefrenceCode";
            DataSet ds2 = new DataSet();
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query1))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@user_code", prm.user_code);
                    cmd.Parameters.AddWithValue("@ecom", prm.ecomCode);
                    cmd.Parameters.AddWithValue("@pack", prm.packSize);
                    cmd.Parameters.AddWithValue("@qty", decryptedQty);
                    cmd.Parameters.AddWithValue("@RefrenceCode", prm.RefrenceCode);

                    con.Open();
                    int iii = cmd.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con.Close();
                }
            }

            DataSet ds3 = new DataSet();
            string query2 = @"
                UPDATE a 
                SET a.qty = @qty 
                FROM tbl_allpaints_suggested_products a 
                WHERE TRY_CAST(user_code AS int) = @user_code 
                AND ecom = @ecom
                AND pack = @pack
                AND RefrenceCode = @RefrenceCode";
            // string query2 = "update a set a.qty =" + prm.qty + " from tbl_allpaints_suggested_products a where TRY_CAST(user_code as int) = " + "'" + prm.user_code + "' and materialCode = " + "'" + prm.unique_suggest + "'";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query2))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@user_code", prm.user_code);
                    cmd.Parameters.AddWithValue("@ecom", prm.ecomCode);
                    cmd.Parameters.AddWithValue("@pack", prm.packSize);
                    cmd.Parameters.AddWithValue("@qty", decryptedQty);
                    cmd.Parameters.AddWithValue("@RefrenceCode", prm.RefrenceCode);

                    con.Open();
                    int iii = cmd.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con.Close();
                }
            }
            var response1 = new DTO.ApiGetResponse
            {
                Status = 1,
                Message = "Cart updated successfully."
            };
            string json = JsonConvert.SerializeObject(response1);
            var encryptCartDtls1 = AesEncryption.Encrypt(json);
            return encryptCartDtls1;    
        }


        public async Task<string> cart(string user_code, string ecomCode, string packSize, string RefrenceCode)
        {

                DataSet ds1 = new DataSet();
                string query = "delete from tbl_allpaints_user_cart where TRY_CAST(user_code as int) = " + "'" + user_code + "' and ecom = " + "'" + ecomCode + "' and pack = " + "'" + packSize + "' and RefrenceCode =" + "'" + RefrenceCode + "'";
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        int iii = cmd.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con.Close();
                    }
                }

                string query1 = "delete from tbl_allpaints_compare_products where TRY_CAST(user_code as int) = " + "'" + user_code + "' and ecom = " + "'" + ecomCode + "' and pack = " + "'" + packSize + "' and RefrenceCode =" + "'" + RefrenceCode + "'";
                DataSet ds2 = new DataSet();
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(query1))
                    {
                        cmd.Connection = con;
                        con.Open();
                        int iii = cmd.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con.Close();
                    }
                }


                DataSet ds3 = new DataSet();
                string query2 = "delete from tbl_allpaints_suggested_products where TRY_CAST(user_code as int) = " + "'" + user_code + "' and ecom = " + "'" + ecomCode + "'and pack = " + "'" + packSize + "' and RefrenceCode =" + "'" + RefrenceCode + "'";
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(query2))
                    {
                        cmd.Connection = con;
                        con.Open();
                        int iii = cmd.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con.Close();
                    }
                }

                DataSet ds4 = new DataSet();
                string query4 = "SELECT * FROM tbl_allpaints_user_cart WHERE TRY_CAST(user_code as int) = " + "'" + user_code + "'";
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(query4))
                    {
                        cmd.Connection = con;
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds4);
                        con.Close();
                    }
                }
                string op = JsonConvert.SerializeObject(ds4.Tables[0], Newtonsoft.Json.Formatting.Indented);
                var model = JsonConvert.DeserializeObject<List<Models.cartItem>>(op);

                var response = new DTO.ApiGetResponse
                {
                    Status = 1,
                    Message = "Item removed from your cart.",
                    count = model.Count
                };

                string json = JsonConvert.SerializeObject(response);
                var encryptCartDtls1 = AesEncryption.Encrypt(json);
                return encryptCartDtls1;

        }


        // insert data
        public async Task<string> Cart(Models.createDtls prm)
        {
            try
            {
                DataSet ds11 = new DataSet();
                string query = "SELECT * FROM tbl_allpaints_user_cart WHERE TRY_CAST(user_code as int) = " + "'" + prm.cartItem[0].user_code + "' and materialCode = " + "'" + prm.cartItem[0].materialCode + "'";
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds11);
                        con.Close();
                    }
                }
                string op = JsonConvert.SerializeObject(ds11.Tables[0], Newtonsoft.Json.Formatting.Indented);
                var model = JsonConvert.DeserializeObject<List<Models.cartItem>>(op);

                if (model.Count > 0)
                {
                    if (prm.cartItem.Count > 0)
                    {
                        for (int i = 0; i < prm.cartItem.Count; i++)
                        {
                            DataSet ds1 = new DataSet();
                            string query1 = @"
                        UPDATE a 
                        SET a.qty = a.qty + @qty 
                        FROM tbl_allpaints_user_cart a 
                        WHERE TRY_CAST(user_code AS int) = @user_code 
                        AND materialCode = @materialCode
                        AND RefrenceCode = @RefrenceCode";
                            //string query1 = "update a set a.qty =a.qty + " + prm.cartItem[i].qty + " from tbl_allpaints_user_cart a where TRY_CAST(user_code as int) = " + "'" + prm.cartItem[i].user_code + "' and materialCode = " + "'" + prm.cartItem[i].materialCode + "'";
                            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {

                                using (SqlCommand cmd = new SqlCommand(query1))
                                {
                                    cmd.Connection = con;
                                    cmd.Parameters.AddWithValue("@qty", prm.cartItem[i].qty);
                                    cmd.Parameters.AddWithValue("@user_code", prm.cartItem[i].user_code);
                                    cmd.Parameters.AddWithValue("@materialCode", prm.cartItem[i].materialCode);
                                    cmd.Parameters.AddWithValue("@RefrenceCode", prm.cartItem[i].RefrenceCode);
                                    con.Open();
                                    int iii = cmd.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con.Close();
                                }
                            }
                        }
                    }

                    if (prm.suggestItem.Count > 0)
                    {
                        for (int i = 0; i < prm.suggestItem.Count; i++)
                        {
                            DataSet ds1 = new DataSet();
                            string query1 = @"
                        UPDATE a 
                        SET a.qty = a.qty + @qty 
                        FROM tbl_allpaints_suggested_products a 
                        WHERE TRY_CAST(user_code AS int) = @user_code 
                        AND materialCode = @materialCode
                        AND RefrenceCode = @RefrenceCode";
                            // string query1 = "update a set a.qty =a.qty +" + prm.suggestItem[i].qty + " from tbl_allpaints_suggested_products a where TRY_CAST(user_code as int) = " + "'" + prm.suggestItem[i].user_code + "' and materialCode = " + "'" + prm.suggestItem[i].materialCode + "'";
                            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {

                                using (SqlCommand cmd = new SqlCommand(query1))
                                {
                                    cmd.Connection = con;
                                    cmd.Parameters.AddWithValue("@qty", prm.suggestItem[i].qty);
                                    cmd.Parameters.AddWithValue("@user_code", prm.suggestItem[i].user_code);
                                    cmd.Parameters.AddWithValue("@materialCode", prm.suggestItem[i].materialCode);
                                    cmd.Parameters.AddWithValue("@RefrenceCode", prm.suggestItem[i].RefrenceCode);
                                    con.Open();
                                    int iii = cmd.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con.Close();
                                }
                            }
                        }
                    }

                    if (prm.compareItem.Count > 0)
                    {
                        for (int i = 0; i < prm.compareItem.Count; i++)
                        {
                            DataSet ds1 = new DataSet();
                            string query1 = @"
                        UPDATE a 
                        SET a.qty = a.qty + @qty 
                        FROM tbl_allpaints_compare_products a 
                        WHERE TRY_CAST(user_code AS int) = @user_code 
                        AND materialCode = @materialCode
                        AND RefrenceCode = @RefrenceCode";
                            //string query1 = "update a set a.qty =a.qty +" + prm.compareItem[i].qty + " from tbl_allpaints_compare_products a where TRY_CAST(user_code as int) = " + "'" + prm.compareItem[i].user_code + "' and materialCode = " + "'" + prm.compareItem[i].materialCode + "'";
                            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {

                                using (SqlCommand cmd = new SqlCommand(query1))
                                {
                                    cmd.Connection = con;
                                    cmd.Parameters.AddWithValue("@qty", prm.compareItem[i].qty);
                                    cmd.Parameters.AddWithValue("@user_code", prm.compareItem[i].user_code);
                                    cmd.Parameters.AddWithValue("@materialCode", prm.compareItem[i].materialCode);
                                    cmd.Parameters.AddWithValue("@RefrenceCode", prm.compareItem[i].RefrenceCode);
                                    con.Open();
                                    int iii = cmd.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con.Close();
                                }
                            }
                        }
                    }

                }
                else
                {
                    if (prm.cartItem.Count > 0)
                    {
                        for (int i = 0; i < prm.cartItem.Count; i++)
                        {
                            DataSet ds = new DataSet();
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {
                                string query1 = "insert into tbl_allpaints_user_cart(user_code,materialName,materialCode,pack,price," +
                                    "GSTPrice,color,qty,stock,img,GstPer,paints,ecom,Updated_price,GSTUpdated_price,Offer,RefrenceCode)" +
                                    " values(@user_code,@materialName,@materialCode,@pack,@price,@GSTPrice,@color,@qty,@stock,@img,@GstPer," +
                                    "@paints,@ecom,@Updated_price,@GSTUpdated_price,@Offer,@RefrenceCode)";
                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@user_code", prm.cartItem[i].user_code);
                                    cmd1.Parameters.AddWithValue("@materialName", prm.cartItem[i].materialName ?? "");
                                    cmd1.Parameters.AddWithValue("@materialCode", prm.cartItem[i].materialCode ?? "");
                                    cmd1.Parameters.AddWithValue("@pack", prm.cartItem[i].pack ?? "");
                                    cmd1.Parameters.AddWithValue("@price", prm.cartItem[i].price);
                                    cmd1.Parameters.AddWithValue("@GSTPrice", prm.cartItem[i].GSTPrice);
                                    cmd1.Parameters.AddWithValue("@color", prm.cartItem[i].color ?? "");
                                    cmd1.Parameters.AddWithValue("@qty", prm.cartItem[i].qty);
                                    cmd1.Parameters.AddWithValue("@stock", prm.cartItem[i].stock);
                                    cmd1.Parameters.AddWithValue("@img", prm.cartItem[i].img ?? "");
                                    cmd1.Parameters.AddWithValue("@GstPer", prm.cartItem[i].GstPer);
                                    cmd1.Parameters.AddWithValue("@paints", prm.cartItem[i].paints);
                                    cmd1.Parameters.AddWithValue("@ecom", prm.cartItem[i].ecom ?? "");
                                    cmd1.Parameters.AddWithValue("@Updated_price", prm.cartItem[i].Updated_price);
                                    cmd1.Parameters.AddWithValue("@GSTUpdated_price", prm.cartItem[i].GSTUpdated_price);
                                    cmd1.Parameters.AddWithValue("@Offer", prm.cartItem[i].Offer ?? "");
                                    cmd1.Parameters.AddWithValue("@RefrenceCode", prm.cartItem[i].RefrenceCode);

                                    con1.Open();
                                    int iii = cmd1.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con1.Close();
                                }
                            }

                        }

                    }

                    if (prm.suggestItem.Count > 0)
                    {
                        for (int j = 0; j < prm.suggestItem.Count; j++)
                        {
                            DataSet ds1 = new DataSet();
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {
                                string query1 = "insert into tbl_allpaints_suggested_products(user_code,materialName,materialCode,pack,price," +
                                    "GSTPrice,color,qty,stock,img,GstPer,paints,ecom,Updated_price,GSTUpdated_price,Offer,RefrenceCode)" +
                                    " values(@user_code,@materialName,@materialCode,@pack,@price,@GSTPrice,@color,@qty,@stock,@img,@GstPer," +
                                    "@paints,@ecom,@Updated_price,@GSTUpdated_price,@Offer,@RefrenceCode)";
                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@user_code", prm.suggestItem[j].user_code);
                                    cmd1.Parameters.AddWithValue("@materialName", prm.suggestItem[j].materialName ?? "");
                                    cmd1.Parameters.AddWithValue("@materialCode", prm.suggestItem[j].materialCode ?? "");
                                    cmd1.Parameters.AddWithValue("@pack", prm.suggestItem[j].pack ?? "");
                                    cmd1.Parameters.AddWithValue("@price", prm.suggestItem[j].price);
                                    cmd1.Parameters.AddWithValue("@GSTPrice", prm.suggestItem[j].GSTPrice);
                                    cmd1.Parameters.AddWithValue("@color", prm.suggestItem[j].color ?? "");
                                    cmd1.Parameters.AddWithValue("@qty", prm.suggestItem[j].qty);
                                    cmd1.Parameters.AddWithValue("@stock", prm.suggestItem[j].stock);
                                    cmd1.Parameters.AddWithValue("@img", prm.suggestItem[j].img ?? "");
                                    cmd1.Parameters.AddWithValue("@GstPer", prm.suggestItem[j].GstPer);
                                    cmd1.Parameters.AddWithValue("@paints", prm.suggestItem[j].paints);
                                    cmd1.Parameters.AddWithValue("@ecom", prm.suggestItem[j].ecom ?? "");
                                    cmd1.Parameters.AddWithValue("@Updated_price", prm.suggestItem[j].Updated_price);
                                    cmd1.Parameters.AddWithValue("@GSTUpdated_price", prm.suggestItem[j].GSTUpdated_price);
                                    cmd1.Parameters.AddWithValue("@Offer", prm.suggestItem[j].Offer ?? "");
                                    cmd1.Parameters.AddWithValue("@RefrenceCode", prm.suggestItem[j].RefrenceCode);

                                    con1.Open();
                                    int iii = cmd1.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con1.Close();
                                }
                            }

                        }

                    }

                    if (prm.compareItem.Count > 0)
                    {
                        for (int k = 0; k < prm.compareItem.Count; k++)
                        {
                            DataSet ds2 = new DataSet();
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {
                                string query1 = "insert into tbl_allpaints_compare_products(user_code,materialName,materialCode,pack,price," +
                                    "GSTPrice,color,qty,stock,img,GstPer,paints,ecom,Updated_price,GSTUpdated_price,Offer,RefrenceCode)" +
                                    " values(@user_code,@materialName,@materialCode,@pack,@price,@GSTPrice,@color,@qty,@stock,@img,@GstPer," +
                                    "@paints,@ecom,@Updated_price,@GSTUpdated_price,@Offer,@RefrenceCode)";
                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@user_code", prm.compareItem[k].user_code);
                                    cmd1.Parameters.AddWithValue("@materialName", prm.compareItem[k].materialName ?? "");
                                    cmd1.Parameters.AddWithValue("@materialCode", prm.compareItem[k].materialCode ?? "");
                                    cmd1.Parameters.AddWithValue("@pack", prm.compareItem[k].pack ?? "");
                                    cmd1.Parameters.AddWithValue("@price", prm.compareItem[k].price);
                                    cmd1.Parameters.AddWithValue("@GSTPrice", prm.compareItem[k].GSTPrice);
                                    cmd1.Parameters.AddWithValue("@color", prm.compareItem[k].color ?? "");
                                    cmd1.Parameters.AddWithValue("@qty", prm.compareItem[k].qty);
                                    cmd1.Parameters.AddWithValue("@stock", prm.compareItem[k].stock);
                                    cmd1.Parameters.AddWithValue("@img", prm.compareItem[k].img ?? "");
                                    cmd1.Parameters.AddWithValue("@GstPer", prm.compareItem[k].GstPer);
                                    cmd1.Parameters.AddWithValue("@paints", prm.compareItem[k].paints);
                                    cmd1.Parameters.AddWithValue("@ecom", prm.compareItem[k].ecom ?? "");
                                    cmd1.Parameters.AddWithValue("@Updated_price", prm.compareItem[k].Updated_price);
                                    cmd1.Parameters.AddWithValue("@GSTUpdated_price", prm.compareItem[k].GSTUpdated_price);
                                    cmd1.Parameters.AddWithValue("@Offer", prm.compareItem[k].Offer ?? "");
                                    cmd1.Parameters.AddWithValue("@RefrenceCode", prm.compareItem[k].RefrenceCode);

                                    con1.Open();
                                    int iii = cmd1.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con1.Close();
                                }
                            }
                        }

                    }


                }

                DataSet ds12 = new DataSet();
                string query2 = "SELECT * FROM tbl_allpaints_user_cart WHERE TRY_CAST(user_code as int) = " + "'" + prm.cartItem[0].user_code + "'";
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(query2))
                    {
                        cmd.Connection = con;
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds12);
                        con.Close();
                    }
                }
                string op1 = JsonConvert.SerializeObject(ds12.Tables[0], Newtonsoft.Json.Formatting.Indented);
                var model1 = JsonConvert.DeserializeObject<List<Models.cartItem>>(op1);

                var response1 = new DTO.ApiGetResponse
                {
                    Status = 0,
                    Message = "Item added to cart successfully",
                        count = model1.Count
                    };
                string json = JsonConvert.SerializeObject(response1);
                var encryptCartDtls1 = AesEncryption.Encrypt(json);
                return encryptCartDtls1;

            }
            catch (Exception ex)
            {
                var response = new DTO.ApiGetResponse
                {
                    Status = 0,
                    Message = "Something went wrong. Please try again later.",
                    Error = ex.Message
                };
                string json = JsonConvert.SerializeObject(response);
                var encryptCartDtls1 = AesEncryption.Encrypt(json);
                return encryptCartDtls1;
            }
        }


        public async Task<string> GetALPNProducts_DoctypeNew(dynamic prm1)
        {
            string json = prm1.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);


            DataSet ds = new DataSet();
            string query = "sp_ShopProductmaster";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;

                    con.Open();


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;
        }

        public async Task<string> GetAllpaintprofileprocess(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "sp_AllPaints_profile_process";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;
            // return new JsonResult(op);
        }

        public async Task<string> getAllpaintsrgbcolor(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<DTO.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "sp_get_Allpaints_rgb_color";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;

        }

        public async Task<string> FetchCustomerDtls(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "sp_get_fetch_customer_details";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;

        }

        public Task<string> cart(DTO.updateCart prm)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AllPaintsOrderData(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "SP_get_ALPN_Franchise_OrderBooking";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;

        }

        public async Task<string> GatwayPaymentProcess(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            string prefix = "ORDID";
            var unixTimestamp = DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds();
            string merchantOrderId = $"{prefix}{unixTimestamp}";

            string tokenUrl = "https://api.phonepe.com/apis/identity-manager/v1/oauth/token";

            var client = new HttpClient();

            // Set content-type header
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Prepare form data
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", "SU2507151940243387572803"),
                    new KeyValuePair<string, string>("client_secret", "91a3ca13-8cf4-4057-92c0-5a4918d9d104"),
                    new KeyValuePair<string, string>("client_version", "1") // Required by PhonePe
            });

            // Send POST request
            var response = await client.PostAsync(tokenUrl, content);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            var result = JObject.Parse(jsonResponse);
            var items1 = result["access_token"];

            string sd = "{\"statusCode\":100,\"msg\":\"Success\",\"error\":[],\"data\":" + " '" + items1 + "' " + "}";

            var model2 = JsonConvert.DeserializeObject<PhonePeResponse>(sd);
            var client1 = new HttpClient();
            client1.DefaultRequestHeaders.Add("Authorization", "O-Bearer " + model2.Data);
            //client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("O-Bearer", model2.Data);
            int amount = int.Parse(prm.filtervalue1);

            var paymentDtls = new PaymentRequest
            {
                merchantOrderId = prm.filtervalue2,
                amount = 100,
                expireAfter = 1200,
                metaInfo = new metaInfo
                {
                    udf1 = "additional-information-1",
                    udf2 = "additional-information-2",
                    udf3 = "additional-information-3",
                    udf4 = "additional-information-4",
                    udf5 = "additional-information-5"
                },
                paymentFlow = new paymentFlow
                {
                    type = "PG_CHECKOUT",
                    message = "Payment message used for collect requests",
                    merchantUrls = new merchantUrls
                    {
                        redirectUrl = "https://shop.allpaints.in/#/layout/main/cart"
                    }
                }
            };

            var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(paymentDtls);
            var data2 = new System.Net.Http.StringContent(json2, Encoding.UTF8, "application/json");

            string paymentUrl = "https://api.phonepe.com/apis/pg/checkout/v2/pay";
            HttpResponseMessage response1 = await client1.PostAsync(paymentUrl, data2);

            string jsonResponse1 = await response1.Content.ReadAsStringAsync();

            var result1 = JsonConvert.DeserializeObject<PhonePeReturn>(jsonResponse1);

            DataSet ds = new DataSet();
            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                string query1 = "insert into tbl_gatewayPayments(orderId,state,expireAt,redirectUrl,merchantOrderId,amount," +
                    "CreatedDate,CreatedBy)" +
                    " values(@orderId,@state,@expireAt,@redirectUrl,@merchantOrderId,@amount,@CreatedDate,@CreatedBy)";
                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                {
                    cmd1.Parameters.AddWithValue("@orderId", result1.orderId);
                    cmd1.Parameters.AddWithValue("@state", result1.state);
                    cmd1.Parameters.AddWithValue("@expireAt", result1.expireAt);
                    cmd1.Parameters.AddWithValue("@redirectUrl", result1.redirectUrl);
                    cmd1.Parameters.AddWithValue("@merchantOrderId", prm.filtervalue2);
                    cmd1.Parameters.AddWithValue("@amount", amount);
                    cmd1.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@CreatedBy", prm.filtervalue3);

                    con1.Open();
                    int iii = cmd1.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con1.Close();
                }
            }
 
            var response2 = new paymentResponse
            {
                statusCode = 200,
                msg = "Success",
                payUrl = result1.redirectUrl
            };
            string json1 = JsonConvert.SerializeObject(response2);
            var encryptCartDtls1 = AesEncryption.Encrypt(json1);
            return encryptCartDtls1;

        }

        public async Task<string> GatwayPaymentDtls(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            string tokenUrl = "https://api.phonepe.com/apis/identity-manager/v1/oauth/token";

            var client = new HttpClient();

            // Set content-type header
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Prepare form data
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", "SU2507151940243387572803"),
                    new KeyValuePair<string, string>("client_secret", "91a3ca13-8cf4-4057-92c0-5a4918d9d104"),
                    new KeyValuePair<string, string>("client_version", "1") // Required by PhonePe
            });

            // Send POST request
            var response = await client.PostAsync(tokenUrl, content);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            var result = JObject.Parse(jsonResponse);
            var items1 = result["access_token"];

            string sd = "{\"statusCode\":100,\"msg\":\"Success\",\"error\":[],\"data\":" + " '" + items1 + "' " + "}";

            var model2 = JsonConvert.DeserializeObject<PhonePeResponse>(sd);
            var client1 = new HttpClient();
            client1.DefaultRequestHeaders.Add("Authorization", "O-Bearer " + model2.Data);



            string paymentUrl = "https://api.phonepe.com/apis/pg/checkout/v2/order/" + prm.filtervalue1 + "/status";
            var response3 = await client1.GetAsync(paymentUrl);
            HttpResponseMessage response1 = await client1.GetAsync(paymentUrl);


            string jsonResponse1 = await response1.Content.ReadAsStringAsync();

            var result1 = JsonConvert.DeserializeObject<paymentStatusResponse>(jsonResponse1);
            string bankResponseJson = JsonConvert.SerializeObject(result1);

            DataSet ds = new DataSet();

            string bankId = "";
            string bankTransactionId = "";
            string paymentMode = "";

            if ((result1.paymentDetails != null) && (result1.paymentDetails.Count > 0) && (result1.paymentDetails[0].splitInstruments != null)
                && (result1.paymentDetails[0].splitInstruments != null) && (result1.paymentDetails[0].splitInstruments[0].instrument != null)
                && (result1.paymentDetails[0].splitInstruments[0].instrument.bankId != null))
            {
                bankId = result1.paymentDetails[0].splitInstruments[0].instrument.bankId;
            }

            if ((result1.paymentDetails != null) && (result1.paymentDetails.Count > 0) && (result1.paymentDetails[0].splitInstruments != null)
            && (result1.paymentDetails[0].splitInstruments != null) && (result1.paymentDetails[0].splitInstruments[0].instrument != null)
            && (result1.paymentDetails[0].splitInstruments[0].instrument.bankTransactionId != null))
            {
                bankTransactionId = result1.paymentDetails[0].splitInstruments[0].instrument.bankTransactionId;
            }
            if((result1.paymentDetails != null) && (result1.paymentDetails.Count > 0))
            {
                paymentMode = result1.paymentDetails[0].paymentMode;
            }
            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                string query1 = "update tbl_gatewayPayments set state=@state,paymentMode=@paymentMode,bankId=@bankId," +
                    "bankResponse=@bankResponse,bankTransactionId=@bankTransactionId where merchantOrderId=@merchantOrderId";
                //string query1 = "insert into employeeotp values(@empotp,@empcode";
                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                {

                    cmd1.Parameters.AddWithValue("@merchantOrderId", prm.filtervalue1);
                    cmd1.Parameters.AddWithValue("@state", result1.state);
                    cmd1.Parameters.AddWithValue("@paymentMode", paymentMode);
                    cmd1.Parameters.AddWithValue("@bankId", bankId);
                    cmd1.Parameters.AddWithValue("@bankTransactionId", bankTransactionId);
                    cmd1.Parameters.AddWithValue("@bankResponse", bankResponseJson);

                    con1.Open();
                    int iii = cmd1.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con1.Close();
                }
            }
            var response2 = new paymentResponse
            {
                statusCode = 200,
                msg = "Success",
                Status = result1.state
            };
            string json1 = JsonConvert.SerializeObject(response2);
            var encryptCartDtls1 = AesEncryption.Encrypt(json1);
            return encryptCartDtls1;
        }

        public async Task<string> PainterGenerateOtp(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prsModel = JsonConvert.DeserializeObject<DTO.Param>(dcriyptData);
            Username = "sureshbv@sheenlac.in";
            Password = "admin123";

            Token token = new Token();
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client1 = new HttpClient(handler);
            var RequestBody = new Dictionary<string, string>
                 {
                 {"username", Username},
                 {"password", Password},
                 };
            var tokenResponse = client1.PostAsync(baseAddress, new FormUrlEncodedContent(RequestBody)).Result;

            if (tokenResponse.IsSuccessStatusCode)
            {
                var JsonContent = tokenResponse.Content.ReadAsStringAsync().Result;

                JObject studentObj = JObject.Parse(JsonContent);

                var result = JObject.Parse(JsonContent);   //parses entire stream into JObject, from which you can use to query the bits you need.
                var items = result["data"].Children().ToList();   //Get the sections you need and save as enumerable (will be in the form of JTokens)

                token.access_token = (string)items[0];
                token.Error = null;
            }
            else
            {
                token.Error = "Not able to generate Access Token Invalid usrename or password";
            }

            client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);


            // var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            //  var data1 = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            //dev
            //var url1 = "http://13.233.6.115/api/v2/paintersReport/allPainter";
            // Prod
            var url1 = "http://13.234.246.143/api/v2/paintersReport/allPainter";

            var SaveRequestBody1 = new Dictionary<string, string>
                 {
                 { "filterValue1",prsModel.filtervalue1},
                 {"filterValue5","Check_Painter"}
                 };

            var json1 = Newtonsoft.Json.JsonConvert.SerializeObject(SaveRequestBody1);
            var data11 = new System.Net.Http.StringContent(json1, Encoding.UTF8, "application/json");


            var response11 = await client1.PostAsync(url1, data11);
            string result7 = response11.Content.ReadAsStringAsync().Result;
            var jsonString2 = JObject.Parse(result7);
            var jsonString21 = Newtonsoft.Json.JsonConvert.SerializeObject(jsonString2);
            var model = JsonConvert.DeserializeObject<PainterDtls>(jsonString21);

            if (model.Data.Count == 0)
            {
                var response2 = new ApiResponse
                {
                    Status = 200,
                    Message = "Please first register the painter"
                };
                string json2 = JsonConvert.SerializeObject(response2);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;

            }
            else
            {
                string sd = Convert.ToString(prsModel.filtervalue1);
                MisResponseStatus responsestatus = new MisResponseStatus();
                HttpResponseMessage response1 = new HttpResponseMessage();
                string responseJson = string.Empty;

                var Starttime = DateTime.Now;
                DateTime now = DateTime.Now;
                DateTime EndTime = now.AddMinutes(15);

                string mob = string.Empty;
                string dsquery = "sp_Get_OTPcode_generate";
                DataSet ds1 = new DataSet();
                using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    using (SqlCommand cmd = new SqlCommand(dsquery))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FilterValue1", prsModel.filtervalue1);
                        cmd.Parameters.AddWithValue("@FilterValue2", prsModel.filtervalue2);
                        cmd.Parameters.AddWithValue("@FilterValue3", prsModel.filtervalue3);
                        cmd.Parameters.AddWithValue("@FilterValue4", prsModel.filtervalue4);
                        cmd.Parameters.AddWithValue("@FilterValue5", prsModel.filtervalue5);
                        cmd.Parameters.AddWithValue("@FilterValue6", prsModel.filtervalue6);
                        cmd.Parameters.AddWithValue("@FilterValue7", prsModel.filtervalue7);
                        cmd.Parameters.AddWithValue("@FilterValue8", prsModel.filtervalue8);
                        cmd.Parameters.AddWithValue("@FilterValue9", prsModel.filtervalue9);
                        cmd.Parameters.AddWithValue("@FilterValue10", prsModel.filtervalue10);
                        cmd.Parameters.AddWithValue("@FilterValue11", prsModel.filtervalue11);
                        cmd.CommandTimeout = 80000;
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds1);
                        con.Close();
                    }
                }

                string op = JsonConvert.SerializeObject(ds1.Tables[0], Formatting.Indented);
                var model2 = JsonConvert.DeserializeObject<List<DTO.OtpVerifyed>>(op);
                //var model2 = JsonConvert.DeserializeObject<OtpVerifyed>(op);

                Random rnd = new Random();
                int[] intArr = new int[100];

                for (int i = 0; i < intArr.Length; i++)
                {
                    int num = rnd.Next(1, 1000000);
                    intArr[i] = num;
                }


                int maxNum = intArr.Max();

                if (model2 != null)
                {
                    for (var i = 0; i < model2.Count; i++)
                    {
                        if (model2[i].Column1 == "0")
                        {
                            return("OTP Attempt Limit Reached. Please Try Again In 15 Minutes.");
                        }
                        else if ((model2[i].Column1 == "1") || (model2[i].Column1 == "2") || (model2[i].Column1 == "3"))
                        {
                            int count = int.Parse(model2[i].Column1);
                            DataSet ds = new DataSet();
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {

                                string query1 = "update tbl_allpaints_otp_process set otp_code=@otp_code,otp_generatetime=@otp_generatetime,starttime=@starttime,endtime=@endtime,attemt_count=@attemt_count where Mobile_number=@Mobile_number";
                                //string query1 = "insert into employeeotp values(@empcode,@empotp)";
                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@Mobile_number", prsModel.filtervalue1);
                                    cmd1.Parameters.AddWithValue("@otp_code", maxNum);
                                    cmd1.Parameters.AddWithValue("@otp_generatetime", DateTime.Now);
                                    cmd1.Parameters.AddWithValue("@starttime", Starttime);
                                    cmd1.Parameters.AddWithValue("@endtime", EndTime);
                                    cmd1.Parameters.AddWithValue("@attemt_count", int.Parse(model2[i].Column1) + 1);

                                    con1.Open();
                                    int iii = cmd1.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con1.Close();
                                }
                            }
                            // return StatusCode(200, "OTP Generate Successfully");
                        }
                        else if (model2[i].Column1 == "4")
                        {
                            int count = int.Parse(model2[i].Column1);
                            DataSet ds = new DataSet();
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {

                                string query1 = "update tbl_allpaints_otp_process set otp_code=@otp_code,otp_generatetime=@otp_generatetime,starttime=@starttime,endtime=@endtime,attemt_count=@attemt_count where Mobile_number=@Mobile_number";
                                //string query1 = "insert into employeeotp values(@empcode,@empotp)";
                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@Mobile_number", prsModel.filtervalue1);
                                    cmd1.Parameters.AddWithValue("@otp_code", maxNum);
                                    cmd1.Parameters.AddWithValue("@otp_generatetime", DateTime.Now);
                                    cmd1.Parameters.AddWithValue("@starttime", Starttime);
                                    cmd1.Parameters.AddWithValue("@endtime", EndTime);
                                    cmd1.Parameters.AddWithValue("@attemt_count", 1);

                                    con1.Open();
                                    int iii = cmd1.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con1.Close();
                                }
                            }
                            // return StatusCode(200, "OTP Generate Successfully");
                        }
                        else if (model2[i].Column1 == "5")
                        {
                            DataSet ds = new DataSet();
                            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                            {

                                string query1 = "insert into tbl_allpaints_otp_process(Mobile_number,otp_code,otp_generatetime,starttime,endtime,ccreatedby,ccreateddate," +
                                    "modifyby,modifydate,attemt_count) values (@Mobile_number,@otp_code,@otp_generatetime,@starttime,@endtime,@ccreatedby,@ccreateddate," +
                                    "@modifyby,@modifydate,@attemt_count)";


                                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                                {
                                    cmd1.Parameters.AddWithValue("@Mobile_number", prsModel.filtervalue1);
                                    cmd1.Parameters.AddWithValue("@otp_code", maxNum);
                                    cmd1.Parameters.AddWithValue("@otp_generatetime", DateTime.Now);
                                    cmd1.Parameters.AddWithValue("@starttime", Starttime);
                                    cmd1.Parameters.AddWithValue("@endtime", EndTime);
                                    cmd1.Parameters.AddWithValue("@ccreatedby", prsModel.filtervalue2);
                                    cmd1.Parameters.AddWithValue("@ccreateddate", DateTime.Now);
                                    cmd1.Parameters.AddWithValue("@modifyby", prsModel.filtervalue3);
                                    cmd1.Parameters.AddWithValue("@modifydate", DateTime.Now);
                                    cmd1.Parameters.AddWithValue("@attemt_count", 1);

                                    con1.Open();
                                    int iii = cmd1.ExecuteNonQuery();
                                    if (iii > 0)
                                    {
                                        //   return StatusCode(200, prsModel.ndocno);
                                    }
                                    con1.Close();
                                }

                            }
                            //  return StatusCode(200, "OTP Generate Successfully");
                        }
                    }


                   // var url2 = "http://13.233.6.115/api/v2/paintersReport/allPainter";
                    // Prod
                    var url2 = "http://13.234.246.143/api/v2/paintersReport/allPainter";

                    var SaveRequestBody2 = new Dictionary<string, string>
                        {
                        { "filterValue1",prsModel.filtervalue1},
                        { "filterValue4",maxNum.ToString()},
                        { "filterValue6","Add_OTP" }
                        };

                    var json11 = Newtonsoft.Json.JsonConvert.SerializeObject(SaveRequestBody2);
                    var data12 = new System.Net.Http.StringContent(json11, Encoding.UTF8, "application/json");

                    var response2 = await client1.PostAsync(url2, data12);
                    string result2 = response2.Content.ReadAsStringAsync().Result;

                }

                var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08045687509&To=" + sd + "&Body=Your Verification Code is  " + maxNum + " - Sheenlac";
                var client = new HttpClient();

                var byteArray = Encoding.ASCII.GetBytes("44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.PostAsync(url, null);

                var result = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(result);
                responsestatus = new MisResponseStatus { StatusCode = "200", Item = "MSG1001", response = result };

                var response3 = new ApiResponse
                {
                    Status = 200,
                    Message = "OTP Generate Successfully"
                };
                string json2 = JsonConvert.SerializeObject(response3);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;


            }
        }


        public async Task<string> PainterOTPVerify(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prsModel = JsonConvert.DeserializeObject<DTO.Param>(dcriyptData);
            int maxno = 0;
            DataSet ds = new DataSet();
            string dsquery = "sp_get_OTPcode_verify";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(dsquery))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prsModel.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prsModel.filtervalue2);
                    cmd.CommandTimeout = 80000;

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                maxno = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return ("error");
            }
            //return StatusCode(201);
            if (maxno == 0)
            {
                var response2 = new ApiResponse
                {
                    Status = 200,
                    Message = "OTP Invalid"
                };
                string json2 = JsonConvert.SerializeObject(response2);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;
            }
            else if (maxno == 1)
            {
                var response2 = new ApiResponse
                {
                    Status = 200,
                    Message = "OTP Verified"
                };
                string json2 = JsonConvert.SerializeObject(response2);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;
            }
            else if (maxno == 2)
            {
                var response2 = new ApiResponse
                {
                    Status = 200,
                    Message = "OTP Expired"
                };
                string json2 = JsonConvert.SerializeObject(response2);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;
            }
            return("success");
        }


        public async Task<string> customerGenOTP(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<DTO.Param>(dcriyptData);
            DataSet ds1 = new DataSet();
            string query = "SELECT * FROM tbl_mis_ALLP_customer_creation WHERE mobile = " + "'" + prm.filtervalue1 + "'";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@mobile", prm.filtervalue1);

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds1);
                    con.Close();
                }
            }
            string op = JsonConvert.SerializeObject(ds1.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var model = JsonConvert.DeserializeObject<List<DTO.createCustomerMadel>>(op);

            if (model.Count == 0)
            {
                var response11 = new ApiResponse
                {
                    Status = 0,
                    Message = "Mobile Number Not Registered for this CustomerCode"
                };
                string json2 = JsonConvert.SerializeObject(response11);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;
                // return StatusCode(200, response11);

            }
            else
            {
                Random rnd = new Random();
                int[] intArr = new int[100];

                for (int i = 0; i < intArr.Length; i++)
                {
                    int num = rnd.Next(1, 10000);
                    intArr[i] = num;
                }

                int maxNum = intArr.Max();


                DataSet ds = new DataSet();
                using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    //string query1 = "update employeeotp set empotp=@empotp where empcode=@empcode";
                    string query1 = "insert into tbl_mis_ALLP_otp_verify(mobileno,OTP,otp_created_by,otp_created_on,otp_verify,otp_veify_on,status) values(@mobileno,@OTP,@otp_created_by,@otp_created_on,@otp_verify,@otp_veify_on,@status)";
                    using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                    {
                        cmd1.Parameters.AddWithValue("@mobileno", prm.filtervalue1);

                        cmd1.Parameters.AddWithValue("@OTP", maxNum);
                        cmd1.Parameters.AddWithValue("@otp_created_by", prm.filtervalue2 ?? "");
                        cmd1.Parameters.AddWithValue("@otp_created_on", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@otp_verify", "N");
                        cmd1.Parameters.AddWithValue("@otp_veify_on", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@status", "N");

                        con1.Open();
                        int iii = cmd1.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con1.Close();
                    }
                }

                var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08047363322&To=" + prm.filtervalue1 + "&Body=Your Verification Code is  " + maxNum + " - Allpaints.in";
                //var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08045687509&To=" + prm.filtervalue1 + "&Body=Your Verification Code is  " + maxNum + " - Sheenlac";

                var client = new HttpClient();

                var byteArray = Encoding.ASCII.GetBytes("44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.PostAsync(url, null);

                var result = await response.Content.ReadAsStringAsync();


                var response1 = new ApiResponse
                {
                    Status = 0,
                    Message = "OTP Send Successfully"
                };
                string json2 = JsonConvert.SerializeObject(response1);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;

            }

        }

        public async Task<string> GetFranchise_Customer_Creation(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<DTO.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "Sp_get_ALPN_Franchise_Customer_Creation";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;

        }

        public async Task<string> createCustomerInfo(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<DTO.createCustomerMadel>(dcriyptData);

            string taxno = string.Empty;

            string mob = prm.mobile;
            DataSet ds1 = new DataSet();
            string query = "SELECT * FROM tbl_mis_ALLP_customer_creation WHERE mobile = " + "'" + prm.mobile + "'";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@mobile", prm.mobile);

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds1);
                    con.Close();
                }
            }
            string op = JsonConvert.SerializeObject(ds1.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var model = JsonConvert.DeserializeObject<List<DTO.createCustomerMadel>>(op);

            if (model.Count > 0)
            {
                var response11 = new ApiResponse
                {
                    Status = 201,
                    Message = "Already Registered for this CustomerCode"
                };
                string json2 = JsonConvert.SerializeObject(response11);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;

            }
            else
            {
                Random rnd = new Random();
                int[] intArr = new int[100];

                for (int i = 0; i < intArr.Length; i++)
                {
                    int num = rnd.Next(1, 10000);
                    intArr[i] = num;
                }

                int maxNum = intArr.Max();


                taxno = "01" + maxNum.ToString();

                DataSet ds = new DataSet();
                using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    //string query1 = "update employeeotp set empotp=@empotp where empcode=@empcode";
                    string query1 = "insert into tbl_mis_ALLP_otp_verify(mobileno,OTP,otp_created_by,otp_created_on,otp_verify,otp_veify_on,status) values(@mobileno,@OTP,@otp_created_by,@otp_created_on,@otp_verify,@otp_veify_on,@status)";
                    using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                    {
                        cmd1.Parameters.AddWithValue("@mobileno", prm.mobile);

                        cmd1.Parameters.AddWithValue("@OTP", maxNum);
                        cmd1.Parameters.AddWithValue("@otp_created_by", prm.createdBy ?? "");
                        cmd1.Parameters.AddWithValue("@otp_created_on", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@otp_verify", "N");
                        cmd1.Parameters.AddWithValue("@otp_veify_on", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@status", "N");

                        con1.Open();
                        int iii = cmd1.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con1.Close();
                    }
                }

                var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08045687509&To=" + prm.mobile + "&Body=Your Verification Code is  " + maxNum + " - Sheenlac";
                var client = new HttpClient();

                var byteArray = Encoding.ASCII.GetBytes("44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.PostAsync(url, null);

                var result = await response.Content.ReadAsStringAsync();


                DataSet ds3 = new DataSet();
                using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    string query1 = "insert into tbl_mis_ALLP_customer_creation values (@firstName,@lastName,@mobile,@mobile2,@dateOfBirth,@customerCode,@id_proff,@email,@gender,@address,@state,@city,@pincode,@gstNumber,@createdBy,@createdAt,@updatedBy,@updatedAt)";

                    using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                    {

                        cmd1.Parameters.AddWithValue("@firstName", prm.firstName ?? "");
                        cmd1.Parameters.AddWithValue("@lastName", prm.lastName ?? "");
                        cmd1.Parameters.AddWithValue("@mobile", prm.mobile ?? "");
                        cmd1.Parameters.AddWithValue("@mobile2", prm.mobile2 ?? "");
                        cmd1.Parameters.AddWithValue("@dateOfBirth", prm.dateOfBirth);
                        cmd1.Parameters.AddWithValue("@customerCode", prm.customerCode ?? "");
                        cmd1.Parameters.AddWithValue("@id_proff", prm.id_proff ?? "");
                        cmd1.Parameters.AddWithValue("@email", prm.email ?? "");
                        cmd1.Parameters.AddWithValue("@gender", prm.gender ?? "");
                        cmd1.Parameters.AddWithValue("@address", prm.address ?? "");
                        cmd1.Parameters.AddWithValue("@state", prm.state ?? "");
                        cmd1.Parameters.AddWithValue("@city", prm.city ?? "");
                        cmd1.Parameters.AddWithValue("@pincode", prm.pincode ?? "");
                        cmd1.Parameters.AddWithValue("@gstNumber", prm.gstNumber ?? "");
                        cmd1.Parameters.AddWithValue("@createdBy", prm.createdBy ?? "");
                        cmd1.Parameters.AddWithValue("@createdAt", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@updatedBy", prm.updatedBy ?? "");
                        cmd1.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                        cmd1.CommandTimeout = 80000;
                        con1.Open();
                        int iii = cmd1.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con1.Close();
                    }
                }

                var response1 = new ApiResponse
                {
                    Status = 200,
                    Message = "Customer Created Successfully"
                };

                string json2 = JsonConvert.SerializeObject(response1);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;

            }
        }

        public async Task<string> OtpVerifyed(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "sp_get_otp_verify";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }

            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var encryptedJson = AesEncryption.Encrypt(op);

            return encryptedJson;
            // return new JsonResult(op);
        }

        public async Task<string> SaveCustomerCreation(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<DTO.CustomercreationParam>(dcriyptData);

            string taxno = string.Empty;

            string mob = string.Empty;
            DataSet ds1 = new DataSet();
            string dsquery = "sp_get_mis_task_employee_details";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(dsquery))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@employeecode", prm.Customer_Mobile);

                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds1);
                    con.Close();
                }
            }

            if (ds1.Tables[0].Rows.Count > 0)
            {
                string customercode = ds1.Tables[0].Rows[0][0].ToString();
                var response11 = new ApiResponse
                {
                    Status = 201,
                    Message = "Mobile Number Already Registered for this CustomerCode: " + customercode
                };
                string json2 = JsonConvert.SerializeObject(response11);
                var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                return encryptCartDtls1;
            }
            else
            {
                Random rnd = new Random();
                int[] intArr = new int[100];

                for (int i = 0; i < intArr.Length; i++)
                {
                    int num = rnd.Next(1, 10000);
                    intArr[i] = num;
                }

                int maxNum = intArr.Max();
                taxno = "01" + maxNum.ToString();

                if (prm.Customer_pin == "" | prm.Customer_pin == null)
                {
                    prm.Customer_pin = "600119";
                }

                DataSet ds = new DataSet();
                using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
                {

                    //string query1 = "update employeeotp set empotp=@empotp where empcode=@empcode";
                    string query1 = "insert into employeeotp values(@empcode,@empotp)";
                    using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                    {
                        cmd1.Parameters.AddWithValue("@empcode", prm.Customer_Mobile);

                        cmd1.Parameters.AddWithValue("@empotp", maxNum);

                        con1.Open();
                        int iii = cmd1.ExecuteNonQuery();
                        if (iii > 0)
                        {
                            //   return StatusCode(200, prsModel.ndocno);
                        }
                        con1.Close();
                    }
                }

                try
                {
                    var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08045687509&To=" + prm.Customer_Mobile + "&Body=Your Verification Code is  " + maxNum + " - Sheenlac";
                    var client = new HttpClient();

                    var byteArray = Encoding.ASCII.GetBytes("44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var response = await client.PostAsync(url, null);

                    var result = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {

                }
            }

            string unicode = string.Empty;
            string cuserdocno = string.Empty;
            DataSet ds12 = new DataSet();

            int maxno1 = 0;

            string maxno = string.Empty;

            string DMSDISTR = string.Empty;

            DataSet ds3 = new DataSet();
            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                string query1 = "insert into tbl_Franchise_customer_creation_v1 values (@Firstname,@LastName,@Phone_number,@Alternative_phone_number,@Email_id,@Gender,@DateOfBirth,@Pincode,@Customercode,@customername,@Customerstatus,@Distributor_code,@Distributor_Name,@GST_No,@id_proff,@Customer_State,@Customer_city,@Attachment1,@Attachment2,@Attachment3,@cremarks1,@cremarks2,@cremarks3,@cremarks4,@cremarks5,@lcreateddate,@Address,@created_by)";

                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                {
                    cmd1.Parameters.AddWithValue("@Firstname", prm.Firstname ?? "");
                    cmd1.Parameters.AddWithValue("@LastName", prm.LastName ?? "");
                    cmd1.Parameters.AddWithValue("@Phone_number", prm.Customer_Mobile ?? "");
                    cmd1.Parameters.AddWithValue("@Alternative_phone_number", prm.Customer_Mobile2 ?? "");
                    cmd1.Parameters.AddWithValue("@Email_id", prm.Customer_Email ?? "");
                    cmd1.Parameters.AddWithValue("@Gender", prm.Gender ?? "");
                    cmd1.Parameters.AddWithValue("@DateOfBirth", prm.DateOfBirth ?? "");
                    cmd1.Parameters.AddWithValue("@Pincode", prm.Customer_pin ?? "");
                    cmd1.Parameters.AddWithValue("@customername", prm.customername ?? "");
                    cmd1.Parameters.AddWithValue("@Customerstatus", prm.Customerstatus ?? "");
                    cmd1.Parameters.AddWithValue("@Distributor_code", prm.Distributor_Code ?? "");
                    cmd1.Parameters.AddWithValue("@Distributor_Name", prm.Customer_Distributor ?? "");
                    cmd1.Parameters.AddWithValue("@GST_No", prm.Customer_Gst_no ?? "");
                    cmd1.Parameters.AddWithValue("@id_proff", prm.id_proff ?? "");
                    cmd1.Parameters.AddWithValue("@Customer_State", prm.Customer_State ?? "");
                    cmd1.Parameters.AddWithValue("@Customer_city", prm.Customer_city ?? "");
                    cmd1.Parameters.AddWithValue("@Attachment1", prm.Attach1 ?? "");
                    cmd1.Parameters.AddWithValue("@Attachment2", prm.Attach2 ?? "");
                    cmd1.Parameters.AddWithValue("@Attachment3", prm.Attach3 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks1", prm.cremarks1 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks2", prm.cremarks2 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks3", prm.cremarks3 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks4", prm.cremarks4 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks5", prm.cremarks5 ?? "");
                    cmd1.Parameters.AddWithValue("@lcreateddate", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@Address", prm.Address);
                    cmd1.Parameters.AddWithValue("@created_by", prm.created_by);


                    //var client = new RestClient($"https://sap.sheenlac.com:44301/sap/zapi_service/zbp_create_mis?sap-client=500");
                    //client.Authenticator = new HttpBasicAuthenticator("MAPOL_API", QAPassword);

                    var options = new RestClientOptions("https://sap.sheenlac.com:44301/sap/zapi_service/zbp_create_mis?sap-client=500")
                    {
                        Authenticator = new HttpBasicAuthenticator("MAPOL_API", QAPassword)
                    };
                    var client = new RestClient(options);

                    Random rnd = new Random();
                    int card = rnd.Next(52);

                    string RootCustomer = "{\r\n    \"ACCOUNT_TYPE\": \"D\",\r\n    \"CUSTOMER\": {\r\n        \"GENERALDATA\": {\r\n            \"GROUPING\": \"CB01\",\r\n            \"TITLE\": \"Company\",\r\n            \"NAME\": {\r\n                \"NAME1\":\"pm\",\r\n                \"NAME2\": \"\",\r\n                \"NAME3\": \"\",\r\n                \"NAME4\": \"\"\r\n            },\r\n            \"ADDRESSDATA\": {\r\n                \"STREET\": \"NEAR PADMA TALKIES,GOPALA UDUPI\",\r\n                \"ADDR1\": \"\",\r\n                \"ADDR2\": \"\",\r\n                \"ADDR3\": \"\",\r\n                \"HOUSE_NUM\": \"\",\r\n                \"POST_CODE1\": \"577205\",\r\n                \"CITY\": \"SHIMOGA\",\r\n                \"DISTRICT\": \"\",\r\n                \"REGION\": \"KA\",\r\n                \"PO_BOX\": \"\"\r\n            },\r\n            \"COMMUNICATION\": {\r\n                \"MOB_NUMBER\":\"6429783582\",\r\n                \"LANDLINE\": \"6429783582\",\r\n                \"SMTP_ADDR\": \"test@gmail.com\"\r\n            },\r\n            \"GROUPING_CHAR\": \"2007\",\r\n            \"ATTRIBUTES\": {\r\n                \"ATTR1\": \"S1\",\r\n                \"ATTR2\": \"02\",\r\n                \"ATTR3\": \"\",\r\n                \"ATTR4\": \"\",\r\n                \"ATTR5\": \"\",\r\n                \"ATTR6\": \"KA2\",\r\n                \"ATTR7\": \"KA3\",\r\n                \"ATTR8\": \"114\",\r\n                \"ATTR9\": \"\",\r\n                \"ATTR10\": \"\"\r\n            },\r\n            \"TAXNO\":" + prm.Customer_Gst_no + ",\r\n            \"PAN_NO\": \"ABZP278IHUY\"\r\n        },\r\n        \"COMPANYCODE\": {\r\n            \"CUST_TYPE\": \"12\",\r\n            \"ZTERM\": \"\"\r\n        },\r\n        \"SALESORG\": {\r\n            \"KTONR\": \"91005042\"\r\n        }\r\n    },\r\n    \"VENDOR\": {\r\n        \"GENERALDATA\": {\r\n            \"GROUPING\": \"VB01\",\r\n            \"TITLE\": \"Company\",\r\n            \"NAME\": {\r\n                \"NAME1\": \"BERGER PAINTS & POLYMERS PVT LTD\",\r\n                \"NAME2\": \"\",\r\n                \"NAME3\": \"\",\r\n                \"NAME4\": \"\"\r\n            },\r\n            \"ADDRESSDATA\": {\r\n                \"STREET\": \"NEAR PADMA TALKIES,GOPALA UDUPI\",\r\n                \"ADDR1\": \"\",\r\n                \"ADDR2\": \"\",\r\n                \"ADDR3\": \"\",\r\n                \"HOUSE_NUM\": \"\",\r\n                \"POST_CODE1\": \"577205\",\r\n                \"CITY\": \"SHIMOGA\",\r\n                \"DISTRICT\": \"\",\r\n                \"REGION\": \"KA\",\r\n                \"PO_BOX\": \"\"\r\n            },\r\n            \"COMMUNICATION\": {\r\n                \"MOB_NUMBER\": \"9878762531\",\r\n                \"LANDLINE\": \"6429783582\",\r\n                \"SMTP_ADDR\": \"test@gmail.com\"\r\n            },\r\n            \"GROUPING_CHAR\": \"1003\",\r\n            \"PAYMENT_TRANSACTION\": [\r\n                {\r\n                    \"BANK_KEY\": \"HDFC0003660\",\r\n                    \"ACC_NUMBER\": \"50200012679420\",\r\n                    \"CONTROL_KEY\": \"12\"\r\n                }\r\n            ],\r\n            \"ACCOUNT_GROUP\": \"YB01\",\r\n            \"GST_VENDOR_CLASSIFICATION\": \"\",\r\n            \"TAXNO\": \"50200012679420\",\r\n            \"PAN_NO\": \"ALPSZ2978HJ\"\r\n        },\r\n        \"COMPANYCODE\": [\r\n            {\r\n                \"COMPANY_CODE\": \"1000\",\r\n                \"RECONILIATION_ACCT\": \"16128541\",\r\n                \"MINORITY_INDICATOR\": \"S\",\r\n                \"CERT_DATE\": \"28.12.2021\",\r\n                \"PAYMENT_TERMS\": \"NT60\",\r\n                \"TOLERANCE_GROUP\": \"\",\r\n                \"PAYMENT_METHODS\": \"N\",\r\n                \"HOUSE_BANK\": \"4180\",\r\n                \"PAYMENT_BLOCK\": \"\",\r\n                \"WITHHOLDING_TAX\": [\r\n                    {\r\n                        \"WTAX_TYPE\": \"Q1\",\r\n                        \"WTAX_CODE\": \"Q1\"\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"COMPANY_CODE\": \"1400\",\r\n                \"RECONILIATION_ACCT\": \"16128541\",\r\n                \"MINORITY_INDICATOR\": \"S\",\r\n                \"CERT_DATE\": \"28.12.2021\",\r\n                \"PAYMENT_TERMS\": \"NT60\",\r\n                \"TOLERANCE_GROUP\": \"\",\r\n                \"PAYMENT_METHODS\": \"N\",\r\n                \"HOUSE_BANK\": \"2278\",\r\n                \"PAYMENT_BLOCK\": \"\",\r\n                \"WITHHOLDING_TAX\": [\r\n                    {\r\n                        \"WTAX_TYPE\": \" \",\r\n                        \"WTAX_CODE\": \" \"\r\n                    }\r\n                ]\r\n            }\r\n        ],\r\n        \"PURCHASING\": [\r\n            {\r\n                \"PURCHASE_ORG\": \"1000\",\r\n                \"PAYMENT_TERMS\": \"NT60\",\r\n                \"PURCHASE_GRP\": \"PUR\",\r\n                \"PLANNED_DELIVERY_TIME\": 7,\r\n                \"SCHEMA_GRP_SUPPLIER\": \"ZD\"\r\n            },\r\n            {\r\n                \"PURCHASE_ORG\": \"1400\",\r\n                \"PAYMENT_TERMS\": \"NT60\",\r\n                \"PURCHASE_GRP\": \"TEC\",\r\n                \"PLANNED_DELIVERY_TIME\": 1,\r\n                \"SCHEMA_GRP_SUPPLIER\": \"ZD\"\r\n            }\r\n        ]\r\n    }\r\n}\r\n\r\n";

                    string sd = Convert.ToString(RootCustomer);
                    custRoot objroot = new custRoot();
                    objroot.CUSTOMER = new CUSTOMER();
                    //objroot.VENDOR = new VENDOR();
                    objroot.ACCOUNT_TYPE = "D";
                    objroot.CUSTOMER.GENERALDATA = new GENERALDATA();
                    objroot.CUSTOMER.GENERALDATA.TITLE = "1003";
                    objroot.CUSTOMER.GENERALDATA.GROUPING = "AL01";
                    objroot.CUSTOMER.GENERALDATA.NAME = new NAME();
                    objroot.CUSTOMER.GENERALDATA.NAME.NAME1 = prm.Firstname;
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA = new ADDRESSDATA();
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA.STREET = prm.Customer_State;
                    objroot.CUSTOMER.GENERALDATA.TAXNO = prm.Customer_Gst_no;
                    objroot.CUSTOMER.COMPANYCODE = new COMPANYCODE();
                    objroot.CUSTOMER.COMPANYCODE.CUST_TYPE = "17";
                    objroot.CUSTOMER.GENERALDATA.GROUPING_CHAR = "2007";
                    objroot.CUSTOMER.SALESORG = new SALESORG();
                    objroot.CUSTOMER.SALESORG.KTONR = prm.Distributor_Code;
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA.HOUSE_NUM = "";
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA.POST_CODE1 = prm.Customer_pin;
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA.CITY = prm.Customer_city;
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA.DISTRICT = "";
                    objroot.CUSTOMER.GENERALDATA.ADDRESSDATA.REGION = "TN";
                    objroot.CUSTOMER.GENERALDATA.COMMUNICATION = new COMMUNICATION();
                    objroot.CUSTOMER.GENERALDATA.COMMUNICATION.MOB_NUMBER = prm.Customer_Mobile;
                    objroot.CUSTOMER.GENERALDATA.COMMUNICATION.LANDLINE = prm.Customer_Mobile2;
                    objroot.CUSTOMER.GENERALDATA.COMMUNICATION.SMTP_ADDR = prm.Customer_Email ?? "nomail@gmail.com";

                    string op = JsonConvert.SerializeObject(objroot, Newtonsoft.Json.Formatting.Indented);

                    var jsondata = "";
                    var request = new RestRequest(jsondata, Method.Post);
                    request.RequestFormat = DataFormat.Json;
                    // string jss=deb
                    RestResponse response;
                    request.AddJsonBody(op);
                    response = await client.PostAsync(request);            //}
                    string op1 = JsonConvert.SerializeObject(response.Content, Formatting.Indented);

                    dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);

                    string sd5 = "{\"statusCode\":100,\"msg\":\"Success\",\"error\":[],\"data\":" + results + "}";

                    var result = JObject.Parse(sd5);

                    var items = result["data"].Children().ToList();   //Get the sections you need and save as enumerable (will be in 

                    var jsonString2 = Newtonsoft.Json.JsonConvert.SerializeObject(items[0]);

                    var model2 = JsonConvert.DeserializeObject<List<SAPGSTRoot>>(jsonString2);

                    cmd1.Parameters.AddWithValue("@Customercode", model2[0].CUSTOMER.CUSTOMER ?? "");
                    con1.Open();
                    int iii = cmd1.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con1.Close();
                    var response11 = new ApiResponse
                    {
                        Status = 200,
                        Message = "Customer Register Successfully"
                    };
                    string json2 = JsonConvert.SerializeObject(response11);
                    var encryptCartDtls1 = AesEncryption.Encrypt(json2);
                    return encryptCartDtls1;
                }

            }
        }

        public async Task<string> SavePainterCreation(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<DTO.CustomerParam>(dcriyptData);

            string unicode = string.Empty;
            string cuserdocno = string.Empty;
            DataSet ds12 = new DataSet();

            int maxno1 = 0;

            string maxno = string.Empty;

            string DMSDISTR = string.Empty;

            DataSet ds3 = new DataSet();
            using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                string query1 = "insert into tbl_Painter_creation values (@Firstname,@LastName,@Phone_number,@Alternative_phone_number,@Email_id, @Gender, @DateOfBirth,@Pincode,@Dealer_code,@Registration_type,@id_proff,@Attachment1,@Attachment2,@Attachment3,@cremarks1,@cremarks2,@cremarks3,@cremarks4,@cremarks5,@lcreateddate,@created_by)";


                using (SqlCommand cmd1 = new SqlCommand(query1, con1))
                {


                    cmd1.Parameters.AddWithValue("@Firstname", prm.Firstname);
                    cmd1.Parameters.AddWithValue("@LastName", prm.LastName);
                    cmd1.Parameters.AddWithValue("@Phone_number", prm.Phone_number);
                    cmd1.Parameters.AddWithValue("@Alternative_phone_number", prm.Alternative_phone_number ?? "");
                    cmd1.Parameters.AddWithValue("@Email_id", prm.Email_id);

                    cmd1.Parameters.AddWithValue("@Gender", prm.Gender);
                    cmd1.Parameters.AddWithValue("@DateOfBirth", prm.DateOfBirth);
                    cmd1.Parameters.AddWithValue("@Pincode", prm.Pincode ?? "");
                    cmd1.Parameters.AddWithValue("@Dealer_code", prm.Dealer_code ?? "");
                    cmd1.Parameters.AddWithValue("@Registration_type", prm.Registration_type ?? "");
                    cmd1.Parameters.AddWithValue("@id_proff", prm.id_proff ?? "");
                    cmd1.Parameters.AddWithValue("@Attachment1", prm.Attachment1 ?? "");
                    cmd1.Parameters.AddWithValue("@Attachment2", prm.Attachment2 ?? "");
                    cmd1.Parameters.AddWithValue("@Attachment3", prm.Attachment3 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks1", prm.cremarks1 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks2", prm.cremarks2 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks3", prm.cremarks3 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks4", prm.cremarks4 ?? "");
                    cmd1.Parameters.AddWithValue("@cremarks5", prm.cremarks5 ?? "");

                    cmd1.Parameters.AddWithValue("@lcreateddate", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@created_by", prm.created_by);

                    con1.Open();
                    int iii = cmd1.ExecuteNonQuery();
                    if (iii > 0)
                    {
                        //   return StatusCode(200, prsModel.ndocno);
                    }
                    con1.Close();
                }

            }

            dynamic jsonData;

            string data = string.Empty;
            //= JsonConvert.DeserializeObject<dynamic>(jsonData.ToString());

            Username = "sureshbv@sheenlac.in";
            Password = "admin123";

            Token token = new Token();
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            var RequestBody = new Dictionary<string, string>
                {
                {"username", Username},
                {"password", Password},
                };
            var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(RequestBody)).Result;

            if (tokenResponse.IsSuccessStatusCode)
            {
                var JsonContent = tokenResponse.Content.ReadAsStringAsync().Result;

                JObject studentObj = JObject.Parse(JsonContent);

                var result = JObject.Parse(JsonContent);   //parses entire stream into JObject, from which you can use to query the bits you need.
                var items = result["data"].Children().ToList();   //Get the sections you need and save as enumerable (will be in the form of JTokens)

                token.access_token = (string)items[0];
                token.Error = null;
            }
            else
            {
                token.Error = "Not able to generate Access Token Invalid usrename or password";
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);


            var json1 = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var data1 = new System.Net.Http.StringContent(json1, Encoding.UTF8, "application/json");

            //var url = "http://13.233.6.115/api/v2/trainings/trainer/AllPaints";
            //dev
            var url = "http://13.234.246.143/api/v2/trainings/trainer/AllPaints";

            var form = new MultipartFormDataContent();

            // Add string form data
            form.Add(new StringContent(prm.Firstname), "first_name");
            form.Add(new StringContent(prm.LastName), "last_name");
            form.Add(new StringContent(prm.Phone_number), "mobile_number");
            form.Add(new StringContent(prm.Alternative_phone_number), "alternate_mobile_number");
            form.Add(new StringContent(prm.Email_id), "email");
            form.Add(new StringContent(prm.Gender), "gender");
            form.Add(new StringContent(prm.DateOfBirth), "birth_date");
            form.Add(new StringContent(prm.Pincode), "pincode");
            form.Add(new StringContent(prm.id_proff), "proof_type");
            form.Add(new StringContent(prm.Registration_type), "registration_type");

            if((prm.Attachment1 != "") && (prm.Attachment1 != null))
            {
                var filePath = prm.Attachment1;
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                form.Add(fileContent, "photo", Path.GetFileName(filePath));
            }

            if ((prm.Attachment2 != "") && (prm.Attachment2 != null))
            {
                var filePath1 = prm.Attachment1;
                var fileStream1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read);

                var fileContent1 = new StreamContent(fileStream1);
                fileContent1.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                form.Add(fileContent1, "proof1", Path.GetFileName(filePath1));
            }
            if ((prm.Attachment3 != "") && (prm.Attachment3 != null))
            {
                var filePath2 = prm.Attachment1;
                var fileStream2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read);

                var fileContent2 = new StreamContent(fileStream2);
                fileContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                form.Add(fileContent2, "proof2", Path.GetFileName(filePath2));
            }

            // Send a POST request with the form data
            HttpResponseMessage response = await client.PostAsync(url, form);

            string result1 = response.Content.ReadAsStringAsync().Result;

            string json2 = JsonConvert.SerializeObject(result1);
            var encryptCartDtls1 = AesEncryption.Encrypt(json2);
            return encryptCartDtls1;

        }

        public async Task<string> GetFranchisePendingOrder(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prm = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            DataSet ds = new DataSet();
            string query = "sp_get_ALPN_Franchise_Pending_Order";
            using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FilterValue1", prm.filtervalue1);
                    cmd.Parameters.AddWithValue("@FilterValue2", prm.filtervalue2);
                    cmd.Parameters.AddWithValue("@FilterValue3", prm.filtervalue3);
                    cmd.Parameters.AddWithValue("@FilterValue4", prm.filtervalue4);
                    cmd.Parameters.AddWithValue("@FilterValue5", prm.filtervalue5);
                    cmd.Parameters.AddWithValue("@FilterValue6", prm.filtervalue6);
                    cmd.Parameters.AddWithValue("@FilterValue7", prm.filtervalue7);
                    cmd.Parameters.AddWithValue("@FilterValue8", prm.filtervalue8);
                    cmd.Parameters.AddWithValue("@FilterValue9", prm.filtervalue9);
                    cmd.Parameters.AddWithValue("@FilterValue10", prm.filtervalue10);
                    cmd.Parameters.AddWithValue("@FilterValue11", prm.filtervalue11);
                    cmd.Parameters.AddWithValue("@FilterValue12", prm.filtervalue12);
                    cmd.Parameters.AddWithValue("@FilterValue13", prm.filtervalue13);
                    cmd.Parameters.AddWithValue("@FilterValue14", prm.filtervalue14);
                    cmd.Parameters.AddWithValue("@FilterValue15", prm.filtervalue15);
                    cmd.CommandTimeout = 80000;
                    con.Open();


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    con.Close();
                }
            }
            string op = JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
            var model = JsonConvert.DeserializeObject<List<InvoiceVerify>>(op);
            if ((model[0].status == "Invoice Created") || (model[0].status == "Success"))
            {
                if ((prm.filtervalue8 != null) && (prm.filtervalue8 != ""))
                {
                    Username = "sureshbv@sheenlac.in";
                    Password = "admin123";

                    Token token = new Token();
                    HttpClientHandler handler = new HttpClientHandler();
                    HttpClient client = new HttpClient(handler);
                    var RequestBody = new Dictionary<string, string>
                            {
                            {"username", Username},
                            {"password", Password},
                            };
                    var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(RequestBody)).Result;

                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        var JsonContent = tokenResponse.Content.ReadAsStringAsync().Result;

                        JObject studentObj = JObject.Parse(JsonContent);

                        var result = JObject.Parse(JsonContent);   //parses entire stream into JObject, from which you can use to query the bits you need.
                        var items = result["data"].Children().ToList();   //Get the sections you need and save as enumerable (will be in the form of JTokens)

                        token.access_token = (string)items[0];
                        token.Error = null;
                    }
                    else
                    {
                        token.Error = "Not able to generate Access Token Invalid usrename or password";
                    }

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

                    //var url = "http://13.233.6.115/api/v2/paintersReport/allPainter";

                    var url = "http://13.234.246.143/api/v2/paintersReport/allPainter";

                    var SaveRequestBody1 = new Dictionary<string, string>
                    {
                        { "filterValue1",prm.filtervalue8},
                        {"filterValue2",prm.filtervalue9},
                        {"filterValue3",prm.filtervalue10},
                        {"filterValue4",prm.filtervalue11}
                    };

                    var json1 = Newtonsoft.Json.JsonConvert.SerializeObject(SaveRequestBody1);
                    var data2 = new System.Net.Http.StringContent(json1, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, data2);
                    string result7 = response.Content.ReadAsStringAsync().Result;
                    var jsonString2 = JObject.Parse(result7);
                    var jsonString21 = Newtonsoft.Json.JsonConvert.SerializeObject(jsonString2);
                    var model1 = JsonConvert.DeserializeObject<PainterPts>(result7);

                    var obj = new Dictionary<string, string>
                    {
                        { "ptsDtls",model1.Data},
                    };
                    // return StatusCode(200, obj);
                }
            }

            string json2 = JsonConvert.SerializeObject(op);
            var encryptCartDtls1 = AesEncryption.Encrypt(json2);
            return encryptCartDtls1;
        }
     
        public async Task<string> EcommerceWhatsappPdf(dynamic prms)
        {
            string json = prms.ToString();
            var dcriyptData = AesEncryption.Decrypt(json);
            var prsModel = JsonConvert.DeserializeObject<Models.Param>(dcriyptData);

            MisResponseStatus responsestatus = new MisResponseStatus();
            string urlFileName = string.Empty;
            byte[] datautr;
            string result = "";
            ByteArrayContent bytes;

            var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v2/accounts/sheenlac2/messages";
            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes("44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            string datas = @"{
                    ""status_callback"": ""https://hikd.requestcatcher.com/"",
                      ""whatsapp"": {
                        ""messages"": [
                          {
                            ""from"": ""+918047363322"",
                            ""to"": ""7402023513"",
                            ""content"": {
                              ""type"": ""template"",
                              ""template"": {                           
                                ""name"": ""allpaints_1"",
                                ""language"": {
                                  ""policy"": ""deterministic"",
                                  ""code"": ""en""
                                },
                                ""components"": [
                                  {
                                    ""type"": ""header"",
                                    ""parameters"": [
                                      {
                                        ""type"": ""document"",
                                        ""document"": {
                                          ""link"": ""https://portal.allpaints.in/assets/images/files/allpaints.pdf"",
                                          ""filename"": ""allpaints.pdf""
                                        },
                                        ""text"": null
                                      }
                                    ]
                                  },
                                  {
                                    ""type"": ""body"",
                                    ""parameters"": [
                                      {
                                        ""type"": ""text"",
                                        ""document"": null,
                                        ""text"": ""Mohan kumar""
                                      }
                                    ]
                                  }
                                ]
                              }
                            }
                          }
                        ]
                      }
                }";

            Roots myDeserializedClass = JsonConvert.DeserializeObject<Roots>(datas);

            // string utrl = "https://devmisportal.sheenlac.com/assets/images/file1/DISF24ORD8848-1.pdf";
            string utrl = "https://portal.allpaints.in/assets/images/files/" + prsModel.filtervalue3 + "";

            string pdflink = utrl;

            myDeserializedClass.whatsapp.messages[0].content.template.components[0].parameters[0].document.link = pdflink;
            myDeserializedClass.whatsapp.messages[0].content.template.components[0].parameters[0].document.filename = prsModel.filtervalue3;
            //filename
            myDeserializedClass.whatsapp.messages[0].to = prsModel.filtervalue1;
            myDeserializedClass.whatsapp.messages[0].content.template.components[1].parameters[0].text = prsModel.filtervalue2;



            string op = JsonConvert.SerializeObject(myDeserializedClass, Formatting.Indented);


            HttpContent _Body = new StringContent(op);

            string _ContentType = "application/json";

            _Body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);

            var response = await client.PostAsync(url, _Body);

            var result1 = await response.Content.ReadAsStringAsync();

            responsestatus = new MisResponseStatus { StatusCode = "200", Item = "MSG1001", response = result1 };

            string json2 = JsonConvert.SerializeObject(responsestatus);
            var encryptCartDtls1 = AesEncryption.Encrypt(json2);
            return encryptCartDtls1;
        }


    }
}
