using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ManagedWin32
{
    public abstract class MemoryScan<T> where T : IEquatable<T>
    {
        public MemoryScan(Process Process) { reader = new ProcessMemory(Process); }

        #region Constant fields
        //Maximum memory block size to read in every read process.

        //Experience tells me that,
        //if ReadStackSize be bigger than 20480, there will be some problems
        //retrieving correct blocks of memory values.
        protected const int ReadStackSize = 20480;
        #endregion

        //Instance of ProcessMemoryReader class to be used to read the memory.
        protected ProcessMemory reader;

        //New thread object to run the scan in
        protected Thread thread;

        public abstract void Start(T Value);

        public abstract void Cancel();

        #region Events
        public event Action<int> ScanProgressChanged;

        protected void OnScanProgressChanged(int Progress) { if (ScanProgressChanged != null)ScanProgressChanged(Progress); }

        public event Action<int[]> ScanCompleted;

        protected void OnScanCompleted(int[] Value) { if (ScanCompleted != null)ScanCompleted(Value); }

        public event Action ScanCanceled;

        protected void OnScanCancelled() { if (ScanCanceled != null)ScanCanceled(); }
        #endregion
    }

    public class RegularMemoryScan<T> : MemoryScan<T> where T : IEquatable<T>
    {
        //Start and End addresses to be scaned.
        IntPtr baseAddress, lastAddress;

        //Class entry point.
        //The process, StartAddress and EndAdrress will be defined in the class definition.
        public RegularMemoryScan(Process process, int StartAddress, int EndAddress)
            : base(process)
        {
            //Set the Start and End addresses of the scan to what is wanted.
            baseAddress = (IntPtr)StartAddress;
            lastAddress = (IntPtr)EndAddress;//The scan starts from baseAddress,
            //and progresses up to EndAddress.
        }
        
        public override void Start(T Value)
        {
            Cancel();

            thread = new Thread(new ParameterizedThreadStart((o) => Scanner(Value)));

            thread.Start();
        }

        //Cancel the scan started.
        public override void Cancel()
        {
            if (thread != null)
            {
                //If the thread is already defined and is Alive,
                if (thread.IsAlive)
                {
                    OnScanCancelled();

                    //and then abort the thread that scanes the memory.
                    thread.Abort();
                }
            }
        }

        void Scanner(T Value)
        {
            //The difference of scan start point in all loops except first loop,
            //that doesn't have any difference, is type's Bytes count minus 1.
            int BytesCount = typeof(T) == typeof(short) ? 2
                : typeof(T) == typeof(int) ? 4
                : typeof(T) == typeof(long) ? 8
                : 0;

            int arraysDifference = BytesCount - 1;

            //Define a List object to hold the found memory addresses.
            List<int> finalList = new List<int>();

            //Open the pocess to read the memory.
            reader.Open();

            //Calculate the size of memory to scan.
            int memorySize = (int)((int)lastAddress - (int)baseAddress);

            //If more that one block of memory is requered to be read,
            if (memorySize >= ReadStackSize)
            {
                //Count of loops to read the memory blocks.
                int loopsCount = memorySize / ReadStackSize;

                //Look to see if there is any other bytes let after the loops.
                int outOfBounds = memorySize % ReadStackSize;

                //Set the currentAddress to first address.
                int currentAddress = (int)baseAddress;

                //This will be used to check if any bytes have been read from the memory.
                int bytesReadSize;

                //Set the size of the bytes blocks.
                int bytesToRead = ReadStackSize;

                //An array to hold the bytes read from the memory.
                byte[] array;

                for (int i = 0; i < loopsCount; i++)
                {
                    //Calculte and set the progress percentage.
                    OnScanProgressChanged((int)(((double)(currentAddress - (int)baseAddress) / (double)memorySize) * 100d));

                    //Read the bytes from the memory.
                    array = reader.Read((IntPtr)currentAddress, (uint)bytesToRead, out bytesReadSize);

                    //If any byte is read from the memory (there has been any bytes in the memory block),
                    if (bytesReadSize > 0)
                    {
                        //Loop through the bytes one by one to look for the values.
                        for (int j = 0; j < array.Length - arraysDifference; j++)
                        {
                            if (typeof(T) == typeof(short))
                            {
                                if (Value.Equals(BitConverter.ToInt16(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                if (Value.Equals(BitConverter.ToInt32(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                if (Value.Equals(BitConverter.ToInt64(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                        }
                    }
                    //Move currentAddress after the block already scaned, but
                    //move it back some steps backward (as much as arraysDifference)
                    //to avoid loosing any values at the end of the array.
                    currentAddress += array.Length - arraysDifference;

                    //Set the size of the read block, bigger, to  the steps backward.
                    //Set the size of the read block, to fit the back steps.
                    bytesToRead = ReadStackSize + arraysDifference;
                }
                //If there is any more bytes than the loops read,
                if (outOfBounds > 0)
                {
                    //Read the additional bytes.
                    byte[] outOfBoundsBytes = reader.Read((IntPtr)currentAddress, (uint)((int)lastAddress - currentAddress), out bytesReadSize);

                    //If any byte is read from the memory (there has been any bytes in the memory block),
                    if (bytesReadSize > 0)
                    {
                        //Loop through the bytes one by one to look for the values.
                        for (int j = 0; j < outOfBoundsBytes.Length - arraysDifference; j++)
                        {
                            if (typeof(T) == typeof(short))
                            {
                                //If any value is equal to what we are looking for,
                                if (Value.Equals(BitConverter.ToInt16(outOfBoundsBytes, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                //If any value is equal to what we are looking for,
                                if (Value.Equals(BitConverter.ToInt32(outOfBoundsBytes, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                //If any value is equal to what we are looking for,
                                if (Value.Equals(BitConverter.ToInt64(outOfBoundsBytes, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                        }
                    }
                }
            }
            //If the block could be read in just one read,
            else
            {
                //Calculate the memory block's size.
                int blockSize = memorySize % ReadStackSize;

                //Set the currentAddress to first address.
                int currentAddress = (int)baseAddress;

                //Holds the count of bytes read from the memory.
                int bytesReadSize;

                //If the memory block can contain at least one 16 bit variable.
                if (blockSize > BytesCount)
                {
                    //Read the bytes to the array.
                    byte[] array = reader.Read((IntPtr)currentAddress, (uint)blockSize, out bytesReadSize);

                    //If any byte is read,
                    if (bytesReadSize > 0)
                    {
                        //Loop through the array to find the values.
                        for (int j = 0; j < array.Length - arraysDifference; j++)
                        {
                            if (typeof(T) == typeof(short))
                            {
                                if (Value.Equals(BitConverter.ToInt16(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                if (Value.Equals(BitConverter.ToInt32(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                if (Value.Equals(BitConverter.ToInt64(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + (int)currentAddress);
                            }
                        }
                    }
                }
            }
            //Close the handle to the process to avoid process errors.
            reader.Dispose();

            //Prepare the ScanProgressed and set the progress percentage to 100% and raise the event.
            OnScanProgressChanged(100);

            //Prepare and raise the ScanCompleted event.
            OnScanCompleted(finalList.ToArray());
        }
    }

    public class IrregularMemoryScan<T> : MemoryScan<T> where T : IEquatable<T>
    {
        //An array to hold the addresses.
        int[] addresses;

        //Class entry point.
        //The process, StartAddress and EndAdrress will be defined in the class definition.
        public IrregularMemoryScan(Process process, int[] AddressesList)
            : base(process)
        {
            //Take the addresses list and store them in local array.
            addresses = AddressesList;
        }

        public override void Start(T Value)
        {
            Cancel();

            thread = new Thread(new ParameterizedThreadStart((o) => Scanner(Value)));

            thread.Start();
        }

        //Cancel the scan started.
        public override void Cancel()
        {
            if (thread != null)
            {
                //If the thread is already defined and is Alive,
                if (thread.IsAlive)
                {
                    OnScanCancelled();

                    //and then abort the alive thread and so cancel last scan task.
                    thread.Abort();
                }
            }
        }

        void Scanner(T Value)
        {
            uint BytesCount = typeof(T) == typeof(short) ? 2u
                : typeof(T) == typeof(int) ? 4u
                : typeof(T) == typeof(long) ? 8u
                : 0;

            //Define a List object to hold the found memory addresses.
            List<int> finalList = new List<int>();

            //Open the pocess to read the memory.
            reader.Open();

            //This will be used to check if any bytes have been read from the memory.
            int bytesReadSize;

            //An array to hold the bytes read from the memory.
            byte[] array;

            for (int i = 0; i < addresses.Length; i++)
            {
                //Calculte and set the progress percentage.
                OnScanProgressChanged((int)(((double)i / (double)addresses.Length) * 100d));

                //Read the bytes from the memory.
                array = reader.Read((IntPtr)addresses[i], BytesCount, out bytesReadSize);

                //If any byte is read from the memory (there has been any bytes in the memory block),
                if (bytesReadSize > 0)
                {
                    if (typeof(T) == typeof(short))
                    {
                        if (Value.Equals(BitConverter.ToInt16(array, 0)))
                            //add it's memory address to the finalList.
                            finalList.Add(addresses[i]);
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        if (Value.Equals(BitConverter.ToInt32(array, 0)))
                            //add it's memory address to the finalList.
                            finalList.Add(addresses[i]);
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        if (Value.Equals(BitConverter.ToInt64(array, 0)))
                            //add it's memory address to the finalList.
                            finalList.Add(addresses[i]);
                    }
                }
            }
            //Close the handle to the process to avoid process errors.
            reader.Dispose();

            //Prepare the ScanProgressed and set the progress percentage to 100% and raise the event.
            OnScanProgressChanged(100);

            OnScanCompleted(finalList.ToArray());
        }
    }

    public class MemoryFreeze<T> where T : IEquatable<T>
    {
        //Struct with 3 properties to retrieve a memory addresse's information.
        public class MemoryRecords
        {
            public MemoryRecords(int Address, T Value)
            {
                this.Address = Address;
                this.Value = Value;
            }

            //Public properties.
            public int Address { get; internal set; }

            public T Value { get; internal set; }

            public Type Type { get { return typeof(T); } }
        }
     
        //List of memoryRecords to hold the addresses and their related information.
        List<MemoryRecords> records;

        //A ProcessMemoryReader object to write the values to the memory addresses.
        ProcessMemory writer;

        //Timer object to tick and freeze.
        System.Timers.Timer timer;
     
        //a property to retrieve the current list of memory addresses set to be freezed.
        public MemoryRecords[] FreezedMemoryAddresses { get { return records.ToArray(); } }
     
        public MemoryFreeze(Process process)
        {
            timer = new System.Timers.Timer();

            writer = new ProcessMemory(process);

            records = new List<MemoryRecords>();

            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerElapsed);
        }

        //Add a memory address and a 16 bit value to be written in the address.
        public void AddMemoryAddress(int MemoryAddress, T Value) { records.Add(new MemoryRecords(MemoryAddress, Value)); }

        //Start the timer with the given Interval to start looping and so start freezing.
        public void Start(double Interval)
        {
            timer.Interval = Interval;
            timer.Start();
        }

        //Stop the timer and so stop the freezing loops.
        public void Stop() { timer.Stop(); }
        
        //The method will be called in every timer's ticking.
        void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Open the process.
            writer.Open();

            //Loop and set the value of all addresses.
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].Type == typeof(short))
                    writer.Write((IntPtr)records[i].Address, BitConverter.GetBytes((short)(object)records[i].Value));

                else if (records[i].Type == typeof(int))
                    writer.Write((IntPtr)records[i].Address, BitConverter.GetBytes((int)(object)records[i].Value));

                else if (records[i].Type == typeof(long))
                    writer.Write((IntPtr)records[i].Address, BitConverter.GetBytes((long)(object)records[i].Value));
            }
            //Close the handle to the process.
            writer.Dispose();
        }
    }
}