using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using MVC.Models;
using Npgsql;

namespace MVC.Repositories
{
    public class ItemRepository
    {
        private readonly NpgsqlConnection conn;
        public ItemRepository(NpgsqlConnection npgsqlConnection)
        {
            conn=npgsqlConnection;
        }
        public async Task<List<ItemModel>> GetAllItems()
        {
            try
            {
                List<ItemModel> item=new List<ItemModel>();
                await conn.CloseAsync();
                string query="SELECT * from t_item order by c_itemid;";
                var cmd=new NpgsqlCommand(query, conn);
                await conn.OpenAsync();
                var reader=cmd.ExecuteReader();
                while (reader.Read())
                {
                    item.Add(new ItemModel()
                    {
                        Itemid=(int)reader["c_itemid"],
                        Name=(string)reader["c_name"],
                        Category=(string)reader["c_category"],
                        Image=(string)reader["c_image"],
                        UnitCost=(int)reader["c_unitcost"],
                        TotalStock=(int)reader["c_totalstock"],
                        CurrentStock=(int)reader["c_currentstock"],
                    });
                }
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item Error: " + ex.Message);
                await conn.CloseAsync();
                return null;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<ItemModel> GetItemById(int id)
        {
            try
            {
                ItemModel item=null;
                await conn.CloseAsync();
                string query="SELECT * from t_item where c_itemid=@id;";
                var cmd=new NpgsqlCommand(query, conn);
                await conn.OpenAsync();
                cmd.Parameters.AddWithValue("id", id);
                var reader=cmd.ExecuteReader();
                if (reader.Read())
                {
                    item=new ItemModel()
                    {
                        Itemid=(int)reader["c_itemid"],
                        Name=(string)reader["c_name"],
                        Category=(string)reader["c_category"],
                        Image=(string)reader["c_image"],
                        UnitCost=(int)reader["c_unitcost"],
                        TotalStock=(int)reader["c_totalstock"],
                        CurrentStock=(int)reader["c_currentstock"],
                    };
                }
                await conn.CloseAsync();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetOne Item Error: " + ex.Message);
                await conn.CloseAsync();
                return null;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<int> AddItem(ItemModel item)
        {
            try
            {
                await conn.CloseAsync();
                string query=@"INSERT into t_item(c_name, c_category, c_image, 
                            c_unitcost, c_totalstock, c_currentstock) VALUES(@name, @category, @image, 
                            @unitcost, @totalstock, @currentstock);";
                var cmd=new NpgsqlCommand(query, conn);
                await conn.OpenAsync();

                cmd.Parameters.AddWithValue("name", item.Name);
                cmd.Parameters.AddWithValue("category", item.Category);
                cmd.Parameters.AddWithValue("image", item.Image);
                cmd.Parameters.AddWithValue("unitcost", item.UnitCost);
                cmd.Parameters.AddWithValue("totalstock", item.TotalStock);
                cmd.Parameters.AddWithValue("currentstock", item.CurrentStock);

                var rd=cmd.ExecuteNonQuery();
                await conn.CloseAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error: " + ex.Message);
                await conn.CloseAsync();
                return 0;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<int> EditItem(ItemModel item)
        {
            try
            {
                await conn.CloseAsync();
                string query=@"UPDATE t_item SET c_name=@name, c_category=@category, c_image=@image, 
                                c_unitcost=@unitcost, c_totalstock=@totalstock, c_currentstock=@currentstock 
                                WHERE c_itemid=@id;";
                await conn.OpenAsync();
                using var cmd=new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", item.Itemid);
                cmd.Parameters.AddWithValue("name", item.Name);
                cmd.Parameters.AddWithValue("category", item.Category);
                cmd.Parameters.AddWithValue("image", item.Image);
                cmd.Parameters.AddWithValue("unitcost", item.UnitCost);
                cmd.Parameters.AddWithValue("totalstock", item.TotalStock);
                cmd.Parameters.AddWithValue("currentstock", item.CurrentStock);

                var rd=cmd.ExecuteNonQuery();
                await conn.CloseAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Edit Error: " + ex.Message);
                await conn.CloseAsync();
                return 0;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<int> DeleteItem(int id)
        {
            try
            {
                await conn.CloseAsync();
                string query="DELETE from t_item WHERE c_itemid=@id;";
                var cmd=new NpgsqlCommand(query, conn);
                await conn.OpenAsync();
                cmd.Parameters.AddWithValue("id", id);

                var rd=cmd.ExecuteNonQuery();
                await conn.CloseAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete Error: " + ex.Message);
                await conn.CloseAsync();
                return 0;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<int> AddItemOrder(ItemOrderModel itemOrder)
        {
            try
            {
                await conn.CloseAsync();
                string query="INSERT into t_item_order(c_customerid, c_itemid, c_quantity, c_totalcost) VALUES(@customerid, @itemid, @quantity, @totalcost);";
                await conn.OpenAsync();
                var cmd=new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("customerid", itemOrder.Customerid);
                cmd.Parameters.AddWithValue("itemid", itemOrder.Itemid);
                cmd.Parameters.AddWithValue("quantity", itemOrder.Quantity);
                cmd.Parameters.AddWithValue("totalcost", itemOrder.TotalCost);

                var rd=cmd.ExecuteNonQuery();
                int a_stock=itemOrder.a_stock - itemOrder.Quantity;

                string query1="UPDATE t_item SET c_currentstock=@a_stock Where c_itemid=@id;";
                var cmd1=new NpgsqlCommand(query1, conn);
                cmd1.Parameters.AddWithValue("a_stock", a_stock);
                cmd1.Parameters.AddWithValue("id", itemOrder.Itemid);
                var rd2=cmd1.ExecuteNonQuery();

                await conn.CloseAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item Order Error:" + ex.Message);
                await conn.CloseAsync();
                return 0;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<List<vm_OrderModel>> GetItemOrderById(int customerid)
        {
            try
            {
                List<vm_OrderModel> itemOrders=new List<vm_OrderModel>();
                await conn.CloseAsync();
                string query=@"SELECT o.c_orderid, o.c_customerid, o.c_itemid, o.c_quantity, o.c_totalcost, 
                                i.c_name, i.c_image from t_item_order o JOIN t_item i ON o.c_itemid=i.c_itemid WHERE o.c_customerid=@customerid;";
                await conn.OpenAsync();
                var cmd=new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("customerid", customerid);
                var reader=cmd.ExecuteReader();
                while (reader.Read())
                {
                    itemOrders.Add(new vm_OrderModel()
                    {
                        Orderid=(int)reader["c_orderid"],
                        Customerid=(int)reader["c_customerid"],
                        Itemid=(int)reader["c_itemid"],
                        Quantity=(int)reader["c_quantity"],
                        TotalCost=(int)reader["c_totalcost"],
                        ItemName=(string)reader["c_name"],
                        Image=(string)reader["c_image"]
                    });
                }
                await conn.CloseAsync();
                return itemOrders;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetItem Order Error: " + ex.Message);
                await conn.CloseAsync();
                return null;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }
    }
}