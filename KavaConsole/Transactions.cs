using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KavaConsole
{
  
    public class Transaction
    {
        public Header header { get; set; }
        public Data data { get; set; }
   
    }
        public class Header
        {
            public int id { get; set; }
            public string chain_id { get; set; }
            public int block_id { get; set; }
            public DateTime timestamp { get; set; }
     
        }

    public class Data
    {
        public string height { get; set; }
        public string txhash { get; set; }
        public string raw_log { get; set; }
        public Log[] logs { get; set; }
        public string gas_wanted { get; set; }
        public string gas_used { get; set; }
        public Tx tx { get; set; }
        public DateTime timestamp { get; set; }
        public string data { get; set; }
        public string codespace { get; set; }
        public int code { get; set; }
  
    }

    public class Tx
    {
        public string type { get; set; }
        public Value value { get; set; }
  
    }

    public class Value
    {
        public Msg[] msg { get; set; }
        public Fee fee { get; set; }
        public Signature[] signatures { get; set; }
        public string memo { get; set; }
    
    }

    public class Fee
    {
        public Amount[] amount { get; set; }
        public string gas { get; set; }
 
    }

    public class Amount
    {
        public string denom { get; set; }
        public string amount { get; set; }

        //not from json, for special case of swap tx's
        public string denom2;
        public string amount2;

        //handle all the different amount types
        public Amount(Collateral c)
        {
            denom = c.denom;
            amount = c.amount;
            decimals();
        }

        public Amount(Principal p)
        {
            amount = p.amount;
            denom = p.denom;
            decimals();
        }
        public Amount(Payment p)
        {
            amount = p.amount;
            denom = p.denom;
            decimals();
        }

        public Amount(Token_A a)
        {
            amount = a.amount;
            denom = a.denom;
            decimals();
        }
        public Amount(Token_B b)
        {
            amount = b.amount;
            denom = b.denom;
            decimals();
        }
        public Amount(Min_Token_A a)
        {
            amount = a.amount;
            denom = a.denom;
            decimals();
        }
        public Amount(Min_Token_B b)
        {
            amount = b.amount;
            denom = b.denom;
            decimals();
        }
        public Amount(Exact_Token_A a)
        {
            amount = a.amount;
            denom = a.denom;
            decimals();
        }
        public Amount(object amt)
        {
            bool list = false;
            List<Amount> alist;
            Amount a;

            try
            {
                 alist = JsonConvert.DeserializeObject<List<Amount>>(amt.ToString());
                if (alist.Count > 1)
                {
                    Console.Error.WriteLine("Unable to handle multiple amounts in single transaction.");
                }
                if (alist != null)
                {
                    amount = alist[0].amount;
                    denom = alist[0].denom;
                }
                list = true;

            }
            catch
            {

            }


            if (!list) try
            {
                 a = JsonConvert.DeserializeObject<Amount>(amt.ToString());
                
                if (a != null)
                {
                    amount = a.amount;
                    denom = a.denom;
                }
                list = true;

            }
            catch
            {

            }



            //Console.WriteLine(amount+ "   "+denom);


            decimals();
        }
        public Amount(string amt)
        {
            //process amounts from 
            string[] s = amt.Split(",");

            Console.WriteLine("Processing string");

            if (s.Length ==2 )
            Console.WriteLine(s[0]+" : "+s[1]);



            if (s != null)
            {

                char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                //split into amount, denom
                int split1 = s[0].LastIndexOfAny(numbers);

                if (split1 == -1)
                {
                    amount = "";
                    denom = "";
                }
                else
                {
                    Console.WriteLine("processing 1st");
                    Console.WriteLine(s[0]);
                    amount = s[0].Substring(0, split1+1);
                    denom = s[0].Substring(split1 + 1, s[0].Length - split1 - 1);

                    Console.WriteLine(amount + " "+denom);


                }

                if (s.Count() == 2)
                {
                    int split2 = s[1].LastIndexOfAny(numbers);


                    if (split2 == -1)
                    {
                        ;//amount = "";
                        //denom = "";
                    }
                    else
                    {
                        Console.WriteLine("processing 2nd");
                        Amount x = new Amount();
                        x.amount = s[1].Substring(0, split2+1);
                        x.denom = s[1].Substring(split2 + 1, s[1].Length - split2 - 1);
                    
                        amount2 = x.amount;
                        denom2 = x.denom;
                        
                    }
                }
                decimals();
            }
        }

        public Amount(Amount[] a)
        {
            if (a == null|| a.Count()==0)
            {
                
            }
            else if (a.Count() > 1)
            {
                Console.Error.WriteLine("Unable to handle multiple amounts in single transaction.");
                amount = a[0].amount;
                denom = a[0].denom;
            }
            else { 
                
                amount = a[0].amount;
                denom = a[0].denom;
            }
        }

        public Amount(Fee f)
        {
            Amount a =new Amount(f.amount);
            amount = a.amount;
            denom = a.denom;
       //     decimals();
        }
        public Amount(List<Fee> f)
        {
            
            Amount a = new Amount(f[0]);
            amount = a.amount;
            denom = a.denom;
            decimals();
        }
        public Amount() { }

        //not ideal duplicate functions
        public static Amount decimals(Amount a)
        {
            Amount b = new Amount();
            int dec = 0;
            if (a.denom == "ukava")
            {
                b.denom = a.denom.Substring(1);
                dec = 6;
            }
            else if (a.denom == "busd")
            {
                dec = 8;

            }
            else if (a.denom == "busd:usdx"||a.denom=="NULL")
            {
                dec = 7;
            }

            else //ukava,usdx,???
            {
                dec = 6;
            }

            //pad front with zeros
            b.amount = a.amount;
            while (b.amount.Length <= dec)
            {
                b.amount = b.amount.Insert(0, "0");
            }
            //insert decimal
            if (b.amount.Length > dec)
            {
                b.amount = b.amount.Insert((b.amount.Length - dec), ".");
            }

            /*while (amount.Length <= dec)
            {
                amount = amount.Insert(0, "0");
            }
            //insert decimal
            if (amount.Length > dec)
            {
                //end - decimal places
                amount = amount.Insert(((amount.Length) - dec), ".");
            }
            */

            return b;
        }
        public void decimals()
        {
            //dont add more decimals
            if (amount != null || amount != "") { 
            string[] s = amount.Split('.');

            //Todo: handle multiple amounts better
            if (amount2 != null && amount2 != "")
            {
                Amount a = new Amount();
                a.amount = amount2;
                a.denom = denom2;
                a.decimals();
                amount2 = a.amount;
                denom2 = a.denom;
            }

                if (s.Count() == 1)
                {

                    int dec = 0;
                    if (denom == "ukava")
                    {
                        denom = denom.Substring(1);
                        dec = 6;
                    }
                    else if (denom == "busd")
                    {
                        dec = 8;

                    }
                    else if (denom == "busd:usdx"||denom=="NULL")
                    {
                        dec = 7;
                    }
                    else //ukava,usdx,???
                    {
                        dec = 6;
                    }


                    //pad front with zeros

                    while (amount.Length <= dec)
                    {
                        amount = amount.Insert(0, "0");
                    }
                    //insert decimal
                    if (amount.Length > dec)
                    {
                        //end - decimal places
                        amount = amount.Insert(((amount.Length) - dec), ".");
                    }
                }
            }
        }


    }



    public class Msg
    {
        public string type { get; set; }
        public Value1 value { get; set; }
       


    }

    public class Value1
    {
        public string delegator_address { get; set; }
        public string validator_address { get; set; }
        public object amount  { get; set; }

        public string from { get; set; }
        public string to { get; set; }
        public string recipient_other_chain { get; set; }
        public string sender_other_chain { get; set; }
        public string random_number_hash { get; set; }
        public string timestamp { get; set; }
        public string height_span { get; set; }
        public string sender { get; set; }
        public Denoms_To_Claim[] denoms_to_claim { get; set; }
        public string depositor { get; set; }
        public string owner { get; set; }
        public string requester { get; set; }
        public Exact_Token_A exact_token_a { get; set; }
        public Token_B token_b { get; set; }
        public string slippage { get; set; }
        public string deadline { get; set; }
        public string borrower { get; set; }
        public string collateral_type { get; set; }
        public Payment payment { get; set; }
        public Collateral collateral { get; set; }
        public Principal principal { get; set; }
        public string shares { get; set; }
        public Min_Token_A min_token_a { get; set; }
        public Min_Token_B min_token_b { get; set; }
        public Token_A token_a { get; set; }

    }

    public class Exact_Token_A
    {
        public string denom { get; set; }
        public string amount { get; set; }
       
    }

    public class Token_B
    {
        public string denom { get; set; }
        public string amount { get; set; }
     
    }

    public class Payment
    {
        public string denom { get; set; }
        public string amount { get; set; }
      
    }

    public class Collateral
    {
        public string denom { get; set; }
        public string amount { get; set; }
    
    }

    public class Principal
    {
        public string denom { get; set; }
        public string amount { get; set; }
   
    }

    public class Min_Token_A
    {
        public string denom { get; set; }
        public string amount { get; set; }
    
    }

    public class Min_Token_B
    {
        public string denom { get; set; }
        public string amount { get; set; }
    
    }

    public class Token_A
    {
        public string denom { get; set; }
        public string amount { get; set; }
      
    }

    public class Denoms_To_Claim
    {
        public string denom { get; set; }
        public string multiplier_name { get; set; }
     
    }

    public class Signature
    {
        public Pub_Key pub_key { get; set; }
        public string signature { get; set; }
    
    }

    public class Pub_Key
    {
        public string type { get; set; }
        public string value { get; set; }
       
    }

    public class Log
    {
        public int msg_index { get; set; }
        public string log { get; set; }
        public Event[] events { get; set; }
 
    }

    public class Event
    {
        public string type { get; set; }
        public Attribute[] attributes { get; set; }

    

    }

    public class Attribute
    {
        public string key { get; set; }
        public string value { get; set; }


    }
    
}