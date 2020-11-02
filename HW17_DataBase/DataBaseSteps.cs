using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using TechTalk.SpecFlow;

namespace HW17_DataBase
{
    [Binding]
    public class DataBaseSteps
    {
        string connectionString = @"Data Source=ALEX-PC;Initial Catalog=TEST_DB;Integrated Security=True";
        string sqlExpression;
        SqlDataAdapter adapter;

        SqlConnection connection;
        SqlCommand sqlCommand;
        int number;

        string firstName;
        string lastName;
        int age;
        string city;

        #region Create person
        [Given(@"person data (.*), (.*), (.*), (.*)")]
        public void GivenPersonData(string firstName, string lastName, int age, string city)
        {
            sqlExpression = String.Format("INSERT INTO Persons (FirstName, LastName, Age, City) VALUES ('{0}', '{1}', {2}, '{3}')",
                 firstName, lastName, age, city);
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = age;
            this.city = city;
        }
        [When(@"send query to data base")]
        public void WhenSendQueryWithDataToDataBase()
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                sqlCommand = new SqlCommand(sqlExpression, connection);
                number = sqlCommand.ExecuteNonQuery();
            }
        }
        [Then(@"new user created with person data")]
        public void ThenNewUserCreatedWithPersonData()
        {
            sqlExpression = "SELECT * FROM Persons WHERE ID = (SELECT MAX(ID) FROM Persons)";
            DataSet dataset = GetDataSet();
            Assert.AreEqual(firstName, dataset.Tables[0].Rows[0].ItemArray[1]);
            Assert.AreEqual(lastName, dataset.Tables[0].Rows[0].ItemArray[2]);
            Assert.AreEqual(age, dataset.Tables[0].Rows[0].ItemArray[3]);
            Assert.AreEqual(city, dataset.Tables[0].Rows[0].ItemArray[4]);
        }
        #endregion

        #region Update person
        [Given(@"update person data (.*), (.*), (.*), (.*)")]
        public void GivenUpdatePersonData(string firstName, string lastName, int age, string city)
        {
            sqlExpression = String.Format("UPDATE Persons SET FirstName='{0}', LastName='{1}', Age='{2}', City='{3}'  WHERE ID=(SELECT MAX(ID) FROM Persons)",
                  firstName, lastName, age, city);
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = age;
            this.city = city;
        }
        #endregion

        #region Create Order
        int sum;
        [Given(@"order sum (.*)")]
        public void GivenOrderSum(int sum)
        {
            sqlExpression = String.Format("INSERT INTO Orders(SUM_order, ID) VALUES({0}, (SELECT MAX(ID) FROM Persons))", sum);
            this.sum = sum;
        }

        [Then(@"new order created")]
        public void ThenNewOrderCreated()
        {
            sqlExpression = "SELECT * FROM Orders WHERE ID = (SELECT MAX(ID) FROM Persons)";
            Assert.AreEqual(sum, GetDataSet().Tables[0].Rows[0].ItemArray[1]);
        }
        #endregion

        #region Delete orders
        [Given(@"query to delete orders")]
        public void GivenDeleteQueryToOrders()
        {
            sqlExpression = String.Format("DELETE FROM Orders WHERE ID=(SELECT MAX(ID) FROM Orders)");
        }

        [Then(@"order is deleted")]
        public void ThenOrderIsDeleted()
        {
            sqlExpression = String.Format("SELECT COUNT(ID) AS Count_ID FROM Orders WHERE ID = (SELECT MAX(ID)+1 FROM Orders)");
            Assert.AreEqual(0, GetDataSet().Tables[0].Rows[0].ItemArray[0]);
        }

        public DataSet GetDataSet()
        {
            DataSet dataset;
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                dataset = new DataSet();
                adapter.Fill(dataset);
            }
            return dataset;
        }
        #endregion

        #region Unique person id
        [Given(@"select all person id")]
        public void GivenSelectAllPersonId()
        {
            sqlExpression = String.Format("SELECT ID FROM Persons");
        }

        [Then(@"all person id is unique")]
        public void ThenAllPersonIdIsUnique()
        {
            DataSet dataset = GetDataSet();
            bool flag = false;
            for (int i = 1; i < dataset.Tables[0].Rows.Count; i++)
            {
                if (dataset.Tables[0].Rows[i].ItemArray[0].Equals(dataset.Tables[0].Rows[i - 1].ItemArray[0]))
                {
                    flag = true;
                    break;
                }
            }
            Assert.IsFalse(flag);
        }
        #endregion

        #region Delete person
        int maxId;
        [Given(@"query to delete person")]
        public void GivenQueryToDeletePerson()
        {
            sqlExpression = String.Format("SELECT MAX(ID) AS max_id FROM Persons");
            maxId = int.Parse(GetDataSet().Tables[0].Rows[0].ItemArray[0].ToString());
            sqlExpression = String.Format("DELETE FROM Persons WHERE ID = {0}", maxId);

        }

        [Then(@"person is deleted")]
        public void ThenPersonIsDeleted()
        {
            sqlExpression = String.Format("SELECT MAX(ID) AS max_id FROM Persons");
            int newMaxId = int.Parse(GetDataSet().Tables[0].Rows[0].ItemArray[0].ToString());
            Assert.AreNotEqual(maxId, newMaxId);
        }
        #endregion

    }
}
