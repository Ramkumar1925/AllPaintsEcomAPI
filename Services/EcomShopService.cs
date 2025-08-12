using System.Data;
using System.Data.SqlClient;
using System.Text;
using AllPaintsEcomAPI.DTO;
using AllPaintsEcomAPI.Helpers;
using AllPaintsEcomAPI.Models;
using AllPaintsEcomAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AllPaintsEcomAPI.Services
{
    public class EcomShopService : EcomShopRepository


    {
        private readonly IConfiguration Configuration;

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


        //public async Task<string> customerGenOTP(dynamic prm)
        //{
        //    DataSet ds1 = new DataSet();
        //    string query = "SELECT * FROM tbl_mis_ALLP_customer_creation WHERE mobile = " + "'" + prm.filtervalue1 + "'";
        //    using (SqlConnection con = new SqlConnection(this.Configuration.GetConnectionString("Database")))
        //    {

        //        using (SqlCommand cmd = new SqlCommand(query))
        //        {
        //            cmd.Connection = con;
        //            cmd.Parameters.AddWithValue("@mobile", prm.filtervalue1);

        //            con.Open();

        //            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //            adapter.Fill(ds1);
        //            con.Close();
        //        }
        //    }
        //    string op = JsonConvert.SerializeObject(ds1.Tables[0], Newtonsoft.Json.Formatting.Indented);
        //    var model = JsonConvert.DeserializeObject<List<DTO.createCustomerMadel>>(op);

        //    if (model.Count == 0)
        //    {
        //        var response11 = new ApiResponse
        //        {
        //            Status = 0,
        //            Message = "Mobile Number Not Registered for this CustomerCode"
        //        };

        //        // return StatusCode(200, response11);

        //    }
        //    else
        //    {
        //        Random rnd = new Random();
        //        int[] intArr = new int[100];

        //        for (int i = 0; i < intArr.Length; i++)
        //        {
        //            int num = rnd.Next(1, 10000);
        //            intArr[i] = num;
        //        }

        //        int maxNum = intArr.Max();


        //        DataSet ds = new DataSet();
        //        using (SqlConnection con1 = new SqlConnection(this.Configuration.GetConnectionString("Database")))
        //        {

        //            //string query1 = "update employeeotp set empotp=@empotp where empcode=@empcode";
        //            string query1 = "insert into tbl_mis_ALLP_otp_verify(mobileno,OTP,otp_created_by,otp_created_on,otp_verify,otp_veify_on,status) values(@mobileno,@OTP,@otp_created_by,@otp_created_on,@otp_verify,@otp_veify_on,@status)";
        //            using (SqlCommand cmd1 = new SqlCommand(query1, con1))
        //            {
        //                cmd1.Parameters.AddWithValue("@mobileno", prm.filtervalue1);

        //                cmd1.Parameters.AddWithValue("@OTP", maxNum);
        //                cmd1.Parameters.AddWithValue("@otp_created_by", prm.filtervalue2 ?? "");
        //                cmd1.Parameters.AddWithValue("@otp_created_on", DateTime.Now);
        //                cmd1.Parameters.AddWithValue("@otp_verify", "N");
        //                cmd1.Parameters.AddWithValue("@otp_veify_on", DateTime.Now);
        //                cmd1.Parameters.AddWithValue("@status", "N");

        //                con1.Open();
        //                int iii = cmd1.ExecuteNonQuery();
        //                if (iii > 0)
        //                {
        //                    //   return StatusCode(200, prsModel.ndocno);
        //                }
        //                con1.Close();
        //            }
        //        }

        //            var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08047363322&To=" + prm.filtervalue1 + "&Body=Your Verification Code is  " + maxNum + " - Allpaints.in";
        //            //var url = "https://44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a@api.exotel.com/v1/Accounts/sheenlac2/Sms/send%20?From=08045687509&To=" + prm.filtervalue1 + "&Body=Your Verification Code is  " + maxNum + " - Sheenlac";

        //            var client = new HttpClient();

        //            var byteArray = Encoding.ASCII.GetBytes("44d5837031a337405506c716260bed50bd5cb7d2b25aa56c:57bbd9d33fb4411f82b2f9b324025c8a63c75a5b237c745a");
        //            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        //            var response = await client.PostAsync(url, null);

        //            var result = await response.Content.ReadAsStringAsync();


        //        var response1 = new ApiResponse
        //        {
        //            Status = 0,
        //            Message = "OTP Send Successfully"
        //        };

        //       // return StatusCode(200, response1);

        //    }

        //}




    }
}
