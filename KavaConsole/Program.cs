// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace KavaConsole;


public sealed class Kava
{
    public string address;
    public List<Transaction> txlist;
    public List<csvTX> csvlist;
    private Kava() { 
        address= "";
        txlist = new List<Transaction>();
        csvlist = new List<csvTX>();
    }

    private static Kava _instance =null;
    public static Kava Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Kava();
            }
            return _instance;

        }
    }
    public static string Address()
    {
        return Instance.address;
    }

}


class Program
{

    static Kava kava = Kava.Instance;

    static void Main(string[] args)
    {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);



        Console.WriteLine("Enter kava address: ");
        kava.address = Console.ReadLine();
        


        
        String path = @"" + home + "\\" +kava.address;
        if (!File.Exists(path))
        {
            string fetchedjson = parseJsonWeb();
            File.WriteAllText(path, fetchedjson);
        }

        parseJsonFile(path);



        processTX();

        string header = "Date,txhash,txid,type,label,received amount,received currency,sent amount, sent currency, fee amount, fee currency,description";

        /*
        
        Console.WriteLine("--------------------Atomic Swaps/Transfers------------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printAtomic();

        
        Console.WriteLine("--------------------Swaps------------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printTrades();
        */
        /*
        Console.WriteLine("--------------------Swap Deposit------------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printSwapDeposits();

        Console.WriteLine("--------------------Swap Withdraws------------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printSwapWithdraws();
        */
        /*
        
        Console.WriteLine("\n\n--------------Hard Deposits ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printHardDeposits();

        Console.WriteLine("\n\n--------------Hard Withdraws ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printHardWithdraws();
        
        
        Console.WriteLine("\n\n--------------Hard Borrow ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printHardBorrow();


        Console.WriteLine("\n\n--------------Hard Repay ---------------------"); 
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printHardRepay();
        
        Console.WriteLine("\n\n--------------CDP Deposit ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printCdpDeposit();

        Console.WriteLine("\n\n--------------CDP Withdraw ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printCdpWithdraw();
        Console.WriteLine("\n\n--------------CDP Create ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printCdpCreate();
        Console.WriteLine("\n\n--------------CDP Borrow ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printCdpBorrow();
        Console.WriteLine("\n\n--------------CDP Repay ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.printCdpRepay();


        */
        Console.WriteLine("\n\n--------------ALL ---------------------");
        Console.WriteLine(header);
        foreach (csvTX tx in kava.csvlist)
            tx.print();
        

        //foreach (Transaction tx in kava.txlist)
        //{
        //csvTX tx2=new csvTX(kava.txlist[0]);
        //kava.csvlist.Add(tx2);

        //}
        // Console.WriteLine(kava.csvlist.Count);
        //    foreach (csvTX tx in kava.csvlist)
        //{

        //  tx.print();

        //}


    }

    static void parseJsonFile(string path)
    {
        string json = File.ReadAllText(path);
        kava.txlist = JsonConvert.DeserializeObject<List<Transaction>>(json);
        Console.WriteLine("Transaction count :"+kava.txlist.Count);
    }

    static string parseJsonWeb()
    {

        //List<Transaction> fetchtxns;

        int lastmsgID = 0;
        int txreq = 50;
        string baseurl = "https://api-kava.cosmostation.io/v1/account/new_txs/" + kava.address + "?limit=" + txreq;
        string start = "&from=";
        bool data = true;


        string fulljson = "[";
        string json = "";
        while (data)
        {
            Console.WriteLine(lastmsgID);
            string url;
            if (lastmsgID != 0)
                url = baseurl + start + lastmsgID;
            else
                url = baseurl;
            json = new WebClient().DownloadString(url);

          
            //check for empty array before parsing
            
            List<Transaction> fetchtx = JsonConvert.DeserializeObject<List<Transaction>>(json);

            if (json.Length<5)
            {
                Console.WriteLine("End");
                data = false;
            }
            else
            {
                if (lastmsgID == 0)
                    fulljson = fulljson + json.Substring(1, json.Length -3);
                else
                    fulljson = fulljson + "," + json.Substring(1, json.Length -3);

                lastmsgID = fetchtx.Last().header.id;
            }
                       

            Thread.Sleep(1000);

        }

        //close json array
        fulljson = fulljson + "]";
        kava.txlist=JsonConvert.DeserializeObject<List<Transaction>>(json);

        return fulljson;// File.WriteAllText(savepath, fulljson);

    }

    public static void processTX()
    {
            List<csvTX> rejected=new List<csvTX>();

        foreach (Transaction tx in kava.txlist)
        {
            csvTX cx = new csvTX(tx);// kava.txlist[0]);

            if (tx.data.code == 5)
            {
                //Console.WriteLine(tx.data.timestamp+","+tx.data.txhash + ", FAILED********?");
                rejected.Add(cx);
            }
            else
            {
                
                kava.csvlist.Add(cx);
            }
       }

        Console.WriteLine("----------------------Reject List-----------------------");
        foreach (csvTX tx in rejected)
            tx.print();
        Console.WriteLine("-----------------------End Reject List----------------");
    }


}