using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KavaConsole
{
    public class csvMsg
    {
        public string type;
        public string label;
        public string RxAmount;
        public string RxCurrency;
        public string TxAmount;
        public string TxCurrency;
        public string FeeAmount;
        public string FeeCurrency;
        public string Description;
        private csvTX parent;

        public csvMsg()
        {
            type = "";
            label = "";
            RxAmount = "";
            RxCurrency = "";
            TxAmount = "";
            TxCurrency = "";
            FeeAmount = "";
            FeeCurrency = "";
            Description = "";

        }
        //clone skeleton constructor
            public csvMsg(csvMsg m)
        {
            parent = m.parent;
            type = m.type;
            label= m.label; 
            Description = m.Description;
        }
        public csvMsg(csvTX parent, Msg m, Data d)
        {
            this.parent=parent;
            processMsg(m, d);

        }
        public void processMsg(Msg m, Data d)
        {
            type = m.type;
            if (type == "bep3/MsgCreateAtomicSwap")
            {
                AtomicSwap(m, d);
            }
            else if (type == "cosmos-sdk/MsgDelegate")
            {
                Delegate(m, d);
            }
            else if (type == "cosmos-sdk/MsgUndelegate")
            {
                Undelegate(m, d);
            }
            else if (type == "/cosmos.staking.v1beta1.MsgBeginRedelegate") {
                Console.WriteLine("handle me");
            
            }
            //cdp
            else if (type == "cdp/MsgCreateCDP")
            {
                Console.WriteLine("Borrow position\n");
                //cdpBorrow(m, d);
                Withdraw(m, d);
            }
            else if (type == "cdp/MsgDeposit")
            {
                Deposit(m, d);
            }
            else if (type == "cdp/MsgWithdraw")
            {
                Withdraw(m, d);
            }
            else if (type == "cdp/MsgDrawDebt")
            {
                Withdraw(m, d);
                //cdpBorrow(m, d);
            }
            else if (type == "cdp/MsgRepayDebt")
            {
                Deposit(m, d);
            }

            //hard
            else if (type == "hard/MsgDeposit")
            {
                Deposit(m, d);
            }
            else if (type == "hard/MsgWithdraw")
            {
                Withdraw(m, d);
            }
            else if (type == "hard/MsgBorrow")
            {
                Withdraw(m, d);
            }
            else if (type == "hard/MsgRepay")
            {
                Deposit(m, d);
            }
            //swap
            else if (type == "swap/MsgDeposit")
            {
                swapDeposit(m, d);
            }
            else if (type == "swap/MsgWithdraw")
            {
                swapWithdraw(m, d);

            }
            else if (type == "swap/MsgSwapExactForTokens")
            {
                swap(m, d);
            }
            else if (type == "incentive/MsgClaimDelegatorReward") {
                Console.WriteLine("fixme");
            }
            else if (type == "incentive/MsgClaimHardReward") { Console.WriteLine("fixme"); 
            }
            else if (type == "incentive/MsgClaimSwapReward") {
                Console.WriteLine("fixme");
            }
            else
            {
                Console.WriteLine("missing tx type: " + label);
            }
        }
        public string print()
        {
            String s = "," + type+","+label + "," + RxAmount + "," + RxCurrency + "," + TxAmount + "," + TxCurrency + "," + FeeAmount + "," + FeeCurrency + "," + Description;
            return s;
        }




        //collect transaction details


        public void Delegate(Msg m, Data d)
        {
            Console.WriteLine("Delegate");
            label = "Sent to Pool";
            Description = "Undelegate";


            Amount a = new Amount(m.value.amount);
            TxAmount = a.amount;
            TxCurrency = a.denom;

            Amount f = new Amount(d.tx.value.fee);
            FeeAmount = f.amount;
            FeeCurrency = f.denom;
   
            Description = "Todo add autoclaimed staking rewards from log entries";

        }


        public void Undelegate(Msg m, Data d)
        {
            Console.WriteLine("UnDelegate, untested");
            label = "From Pool";

            Amount b =new Amount(m.value.amount);
            RxAmount = b.amount;
            RxCurrency = b.denom;

            //Amount f = new Amount(d.tx.value.fee);
            //FeeAmount = f.amount;
            //FeeCurrency = f.denom;
            /*
            int i = 0;
            foreach (Amount fee in d.tx.value.fee.amount)
            {
                if (i > 1)
                {
                    Console.WriteLine("Multiple Fee Amounts");
                }
                Amount c= decimals(fee);
                FeeAmount = c.amount;
                FeeCurrency = c.denom;
                i++;
            }*/
            Description = "Undelegate";

        }

        public void AtomicSwap(Msg m, Data d)
        {
            Console.WriteLine("AtomicSwap");
            Amount a = new Amount(m.value.amount);
            //List<Amount> a = JsonConvert.DeserializeObject<List<Amount>>(m.value.amount.ToString());
            //Amount b = decimals(a[0]);
            //if (a.Count > 1)
            //{
            //  Description = "Multiple tokens, manual processing required";
            //}
            label = "transfer";

            if (m.value.from == Kava.Address())
            {
                TxAmount = a.amount;
                TxCurrency = a.denom;


                Description = "Sending Cross Chain";
                Amount f = new Amount(d.tx.value.fee);
                FeeAmount = f.amount;
                FeeCurrency = f.denom;


            }
            else if (m.value.to == Kava.Address())
            {
                RxAmount = a.amount;
                RxCurrency = a.denom;
                Description = "Receiving CrossChain";
              
                /* Sender pays fee
                Amount f=new Amount(d.tx.value.fee);
                   FeeAmount = f.amount;
                   FeeCurrency = f.denom;
                */
            
            }
            else
            {
                Description = " undefined sender receiver";
            }




        }
        public void Deposit(Msg m, Data d)
        {

            Amount a = null;
            if (type == "cdp/MsgDeposit")
            {
                //not tax reportable
                //a = new Amount(m.value.collateral);
                a = new Amount();
                label = "Sent to Pool";
                Description = "CDP DEP";
            }
            else if (type == "hard/MsgDeposit")
            {
                //not tax reportable
                //                a = new Amount(m.value.amount);
                a = new Amount();
                label = "Sent to Pool";
                Description= "Hard Lending Pool Dep";


            }
            else if(type == "hard/MsgRepay")
            {
                a = new Amount(m.value.amount);

                label = "Loan Repay";
                Description = "Hard Loan Repay";
            }
            else if(type=="cdp/MsgRepayDebt")
            {
               // Console.WriteLine(" CDP Repay");
                a = new Amount(m.value.payment);
                label = "Loan Repay";
                Description = "CDP BURN/ LOAN REPAY";

                //reclaim collateral from msg logs

                if (d.logs.Count() > 0) //no guarantees to be the collateral amount
                    {
                        foreach (Event e in d.logs[0].events)    
                        {
                            if (e.type == "transfer")
                            {

                                if (e.attributes.Count() == 6)
                                {
                                    for (int i = 0; i < e.attributes.Count(); i++)
                                    {
                                        if (e.attributes[i].key == "recipient")
                                        {
                                        
                                           // Console.WriteLine(e.attributes[i].key +" "+ e.attributes[i].value);
                                            if (e.attributes[i].value == Kava.Instance.address)
                                            {

                                                csvMsg col = new csvMsg(this);
                                                //token_b.label = label;
                                                col.label = "CDP Collateral Reclaim";
                                                col.Description = "CDP Collateral, information only, or transferred?";
                                            if ((i + 2) < e.attributes.Count()) //not guaranteed to be correct position
                                            {
                                                if (e.attributes[i + 2].value != null && e.attributes[i + 2].value!= "") {
                                              //      Console.WriteLine(e.attributes[i + 2].value);
                                                    
                                                        Amount c = new Amount(e.attributes[i + 2].value);
                                                        col.RxAmount = c.amount;
                                                        col.RxCurrency = c.denom;
                                                        Console.WriteLine(d.txhash+ " Collateral Rxd: " + col.RxAmount + " , " + col.RxCurrency);
                                                    //    parent.msg.Add(col);
                                                } }

                                            }
                                        }


                                    }


                                }

                            }

                        }
    
                }
                    Console.WriteLine(" CDP Repay End");
                



            }

            TxAmount = a.amount;
            TxCurrency = a.denom;
            //label = "Sent To Pool";

            /*
            int i = 0;
            foreach (Amount fee in d.tx.value.fee.amount)
            {
                Amount c = new Amount(fee);
                if (i > 1)
                {
                    Description = "Multiple Fee Amounts Detected";
                }
                FeeAmount = c.amount;
                FeeCurrency = c.denom;
                i++;
            }*/
        }



        public void Withdraw(Msg m, Data d)
        {

            Amount a = null;
            if (type == "hard/MsgWithdraw")
            {
                a = new Amount();
                //not tax reportable
                //a = new Amount(m.value.amount);
                label = "Received From Pool";
                Description = "Hard Lending Pool WD";


            }
            else if (type == "hard/MsgBorrow")
            {
                a = new Amount(m.value.amount);

                label = "Loan Borrowed";
                Description = "Hard Loan Borrow";
            }
           /* else if (type == "cdp/MsgBorrowDebt")
            {
                a = new Amount(m.value.payment);
                label = "Loan Borrowed";
                Description = "CDP MINT/ LOAN REPAY";
            }*/
            else if( type == "cdp/MsgWithdraw")
            {
                label = "From Pool";
                Description = " CDP POOL WD";
                //  a = new Amount(m.value.collateral);
                //not tax reportable
                a = new Amount();

            }
            else if (type=="cdp/MsgDrawDebt"||type== "cdp/MsgCreateCDP")
            {
                label = "Loan Borrowed";
                Description = "CDP MINT/BORROW";

                a = new Amount(m.value.principal);
                
                if (m.value.collateral != null)
                {
                    csvMsg col = new csvMsg(this);
                    //token_b.label = label;
                    
                    col.label = "CDP Collateral";
                    col.Description = "CDP Collateral, information only, or transferred?";
                    Amount c = new Amount(m.value.collateral);
                    col.TxAmount = c.amount;
                    col.TxCurrency = c.denom;
                   // parent.msg.Add(col);
                    //Console.WriteLine("Collateral: " + c.amount + "  " + c.denom);
                }

            }
            else
            {

                Console.WriteLine("Withdraw");
                label = "From pool";
                Description = "Generic WD";
               // a = new Amount(m.value.amount);
            }

            RxAmount = a.amount;
            RxCurrency = a.denom;
           // label = "Received From Pool";

        }
      
   /*
        public void cdpBorrow(Msg m, Data d)
        {
            Console.WriteLine("cdpBorrow");
            label = "Loan Borrow";
            Description = "CDP MINT/BORROW";


            Amount a = new Amount(m.value.principal);
            RxAmount = a.amount;
            RxCurrency = a.denom;
            //Console.WriteLine("Borrowed: "+a.amount + "  " + a.denom);
            if (m.value.collateral != null)
            {
                csvMsg col = new csvMsg(this);
                //token_b.label = label;
                col.label = "Sent to Pool";
                col.Description = "CDP Collateral, information only, or transferred?";
                Amount c = new Amount(m.value.collateral);
                col.TxAmount = c.amount;
                col.TxCurrency = c.denom;
                parent.msg.Add(col);
                //Console.WriteLine("Collateral: " + c.amount + "  " + c.denom);
            }

        }
 */
        public void swap(Msg m,Data d)
        {
            label = "Exchange";
            Description = " Token Swap";
            if (m.value.exact_token_a != null)
            {
                Amount a=new Amount(m.value.exact_token_a);
                TxAmount = a.amount;
                TxCurrency = a.denom;
            }
            else if (m.value.token_a != null)
            {
                Amount a = new Amount(m.value.token_a);
                TxAmount = a.amount; 
                TxCurrency = a.denom;
            }

            Amount b = new Amount(m.value.token_b);
            RxAmount = b.amount;
            RxCurrency= b.denom;

            //Todo: add tx fees

        }
       

        public void swapDeposit(Msg m, Data d)
        {
            Description = "Swap Pool Deposit ";
            //label= "Liquidity_In";
            label = "Sent to Pool";//"Liquidity_In";
            //search for key shares, pool_id
            Amount shares = new Amount();
            //string pool = "";
            //Amount swp;
            foreach (Log l in d.logs)
            {
                foreach (Event e in l.events)
                {
                    foreach (Attribute a in e.attributes)
                    {
                        if (a.key == "shares")
                        {
                            shares.amount = a.value;
                        }
                        else if (a.key == "pool_id")
                        {
                            shares.denom = "NULL"; //a.value;   koinly tosses transactions for unknown token, replaced with NULL for placeholders
                            Description += a.value;
                        }
                        else if (a.key == "amount")
                        {
                            //   swp = new Amount(a.value);
                            // Console.WriteLine(swp.amount + "," + swp.denom + "," + swp.amount2 + "," + swp.denom2);
                            // Description += "   swap string"+a.value;

                        }
                    }
                }
            }

            Amount x = new Amount(m.value.token_a);
            TxAmount = x.amount;
            TxCurrency = x.denom;


            shares.amount=(long.Parse(shares.amount)/2).ToString();
            shares.decimals();
          //  RxAmount = shares.amount;
          //  RxCurrency = shares.denom;

            shares.decimals();
            // RxAmount = m.value.token_a.amount;

            //create a 2nd msg for 2nd tokens


            csvMsg token_b=new csvMsg(this);
            //token_b.label = label;
            Amount y = new Amount(m.value.token_b);
            token_b.TxAmount = y.amount;
            token_b.TxCurrency = y.denom;
            //token_b.RxAmount = RxAmount;
           // token_b.RxCurrency = RxCurrency;

            parent.msg.Add(token_b);

        }
        public void swapWithdraw(Msg m, Data d)
        {
            Description = "Swap Pool Withdraw ";
            label = "From Pool";
            //search for key shares, pool_id
            Amount shares = new Amount();


            string amount = "";
            foreach (Log l in d.logs)
            {
                foreach (Event e in l.events)
                {
                    foreach (Attribute a in e.attributes)
                    {
                        if (a.key == "shares")
                        {
                            shares.amount = a.value;
                        }
                        else if (a.key == "pool_id")
                        {
                            shares.denom = "NULL"; // a.value;  //koinly discards unrecognized tokens null placeholder, asign to unique NULL# placeholder per pool type
                            Description += a.value;
                        }
                        else if (a.key == "amount")
                        {
                            amount = a.value;

                        }
                    }
                }

            }
            //manual guess of split pool 50:50;
            shares.amount = (long.Parse(shares.amount) / 2).ToString();
            shares.decimals();
           // TxAmount = shares.amount; 
            //TxCurrency = shares.denom;

            //Amount y = new Amount(m.value.min_token_b);
            Amount y = new Amount(amount);
            RxAmount = y.amount;
            RxCurrency = y.denom;


            csvMsg rx = new csvMsg(this);
  //          csvMsg rx2 = new csvMsg(this);
  
          //  Amount x=new Amount(m.value.min_token_a);
           // rx.TxAmount = TxAmount;
           // rx.TxCurrency = TxCurrency;
            rx.RxAmount = y.amount2;
            rx.RxCurrency = y.denom2;

            parent.msg.Add(rx);
            //parent.msg.Add(rx2);

        }


        //end class
    }

    public class csvTX
    {
        public DateTime date;
        public string txhash;
        public int txid;
        public List<csvMsg> msg;
        private Transaction originalTX;
        private List<Msg> msgList;
        public csvTX()
        {
            msg = new List<csvMsg>();
          //  msgList = new List<Msg>();
        }
        
        public csvTX(Transaction tx)
        {
            msg = new List<csvMsg>();
            msgList = new List<Msg>();
            originalTX = tx;
            processTxn(tx);
          //  print();

        }
        public void processTxn(Transaction tx)
        {
            //Console.WriteLine("Processing Transaction");
            date = tx.data.timestamp;
            txhash = tx.data.txhash;
            txid = tx.header.id;
           // Console.WriteLine(date + "," + txhash + "" + txid );


            if (tx.data.tx.value != null)
            {
                if (tx.data.tx.value.msg!=null)
                    foreach (Msg m in tx.data.tx.value.msg)
                    {
                      //  Console.WriteLine("adding "+m.ToString());
                      csvMsg cm=new csvMsg(this, m, tx.data);
                      //  cm.print();
                      msg.Add(cm);
                    }
                else
                {
                    csvMsg cm = new csvMsg();
                    
                    cm.label = "Unknown";
                    msg.Add(cm); //add empty

                    Console.WriteLine("something");
                }
            }
            else
            {
                csvMsg cm=new csvMsg();
                cm.label = "Unknown";
                msg.Add(cm); //add empty
                
                Console.WriteLine("empty");
            }
        }







        //print transaction details


        public void print()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                Console.WriteLine(date + "," + txhash + "," + txid+ m.print());
                //Console.WriteLine(msgList.Count);
            }
        }

        public void printTrades()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "swap/MsgSwapExactForTokens")
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                //Console.WriteLine(msgList.Count);
            }
        }
        public void printAtomic()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "bep3/MsgCreateAtomicSwap")
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                //Console.WriteLine(msgList.Count);
            }
        }
        public void printSwapDeposits()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if ((m.type == "swap/MsgDeposit"))
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                //Console.WriteLine(msgList.Count);
            }
        }
        public void printSwapWithdraws()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if ((m.type == "swap/MsgWithdraw"))
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                //Console.WriteLine(msgList.Count);
            }
        }
        public void printHard()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type.Contains("hard"))
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printHardDeposits()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type=="hard/MsgDeposit")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printHardWithdraws()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "hard/MsgWithdraw")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printHardBorrow()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "hard/MsgBorrow")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printHardRepay()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "hard/MsgRepay")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }


        public void printCdp()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type.Contains("cdp"))
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printCdpDeposit()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "cdp/MsgDeposit")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printCdpWithdraw()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "cdp/MsgWithdraw")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }

        public void printCdpBorrow()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "cdp/MsgDrawDebt")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        public void printCdpRepay()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "cdp/MsgRepayDebt")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }

        public void printCdpCreate()
        {
            //Console.WriteLine("test");
            foreach (csvMsg m in msg)
            {
                if (m.type == "cdp/MsgCreateCDP")
                {
                    Console.WriteLine(date + "," + txhash + "," + txid + m.print());
                    //Console.WriteLine(msgList.Count);
                }
            }
        }
        

    }




}
