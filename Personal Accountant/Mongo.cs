using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using Personal_Accountant.Schemas;
using System.Linq;
using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MongoDB.Bson;
using System.Xml.Linq;
using System.Drawing;

namespace Personal_Accountant
{
    public class mongo
    {
        private const string ConnectionString = "mongodb://127.0.0.1:27017";

        private const string DatabaseName = "Personal_Accountant";
        private const string DaysCollection = "days";


        private IMongoCollection<Schema> ConnectToMongo<Schema>(in string collection)
        {
            var clinet = new MongoClient(ConnectionString);
            var db = clinet.GetDatabase(DatabaseName);
            return db.GetCollection<Schema>(collection);
        }



        #region Day 

        public async Task<List<DaySchema>> GetAllDays()
        {
            var Days = ConnectToMongo<DaySchema>(DaysCollection);
            var results = await Days.FindAsync(_ => true);
            return results.ToList();
        }
        public async Task<List<DaySchema>> GetDay(int date)
        {
            var Day = ConnectToMongo<DaySchema>(DaysCollection);
            var results = await Day.FindAsync(a => a.Date == date);
            return results.ToList();
        }
        public async Task AddDay(DaySchema day)
        {
            var Day = ConnectToMongo<DaySchema>(DaysCollection);
            await Day.InsertOneAsync(day);
            return;
        }
        public Task UpdateDay(DaySchema day)
        {
            var Day = ConnectToMongo<DaySchema>(DaysCollection);
            var fitler = Builders<DaySchema>.Filter.Eq("Id", day.Id);
            return Day.ReplaceOneAsync(fitler, day, new ReplaceOptions { IsUpsert = true });
        }
        public Task DeleteDay(DaySchema day)
        {
            var Day = ConnectToMongo<DaySchema>(DaysCollection);
            return Day.DeleteOneAsync(i => i.Id == day.Id);
        }
        #endregion Transaction

        #region TR 

        public async Task<List<TransSchema>> GetAllTransactionsOfDay(int Day)
        {
            var results =  await GetDay(Day);
            if (results.Count == 0)
                return new List<TransSchema>();
            return results.First().Transactions;
        }

        public async Task<double> GetBalance()
        {
            List<DaySchema> days = await GetAllDays();
            if (days.Count == 0)
                return 0;
            return days.Last().Balance;

        }
        public async Task AddTransaction(TransSchema trans, int Day)
        {
            var results = await GetDay(Day);
            DaySchema day;
            if (results.Count == 0)
            {
                day = new DaySchema();
                day.Transactions = new List<TransSchema>();
                day.Transactions.Add(trans);
                day.Date = Day;
                day.Balance = await GetBalance();
            }
            else
            {
                day = results.First();
            }
            day.Transactions.Add(trans);
            day.Balance += trans.Type == "in" ? trans.Amount : -trans.Amount;
            await UpdateDay(day);
        }

        public async Task DeleteTransaction(TransSchema trans, int Day)
        {
            var results = await GetDay(Day);

            if (results.Count == 0)
                return;

            DaySchema day = results.First();

            if (!day.Transactions.Contains(trans))
                return;
            day.Transactions.Remove(trans);
            day.Balance -= trans.Type == "in" ? trans.Amount : -trans.Amount;
            await UpdateDay(day);
        }

        #endregion

        public async void DeletAll()
        {
            var client = new MongoClient(ConnectionString);
            await client.DropDatabaseAsync(DatabaseName);
        }
    }
}
