using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace testeDisposable
{
    class Program
    {
        public class teste : IDisposable
        {
            public enum estado
            {
                desligado = 0,
                ligado = 1,
                standby = 2
            }
            public void Dispose()
            {
                Console.WriteLine("Dispose");
            }
        }
        static void Main(string[] args)
        {
            
            //Console.WriteLine("INICIOU");

            //using (teste Teste = new teste())
            //{
            //    throw new Exception("Erro");
            //}

            Database database;

            string strCon = @"Data Source=TDS-DSV32\SQLEXPRESS;Initial Catalog=banco_teste;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            database = new SqlDatabase(strCon);

            DbConnection dbConnection = database.CreateConnection();

            dbConnection.Open();

            DbTransaction transaction = dbConnection.BeginTransaction();

            DbCommand dbCommand =  dbConnection.CreateCommand();

            dbCommand.Transaction = transaction;
            dbCommand.CommandText = "select nome from pessoas";


            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.CreateXmlDeclaration("1.0","utf-8","no");
            XmlElement element = xmlDocument.CreateElement("Elemento");
            xmlDocument.AppendChild(element);

            element.AppendChild(xmlDocument.CreateElement("elemento1")).InnerText = "elemento11";
            element.AppendChild(xmlDocument.CreateElement("status")).InnerText = "0";


            var reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                XmlElement eleItens = xmlDocument.CreateElement("ListaNomes");
                eleItens.AppendChild(xmlDocument.CreateElement("Nome")).InnerText = reader["nome"].ToString();

                element.AppendChild(eleItens);
            }

            reader.Close();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create("data.xml", settings);
            
            xmlDocument.Save(writer);

            transaction.Commit();

            dbConnection.Close();
            dbConnection.Dispose();
            transaction.Dispose();
        }
    }
}
