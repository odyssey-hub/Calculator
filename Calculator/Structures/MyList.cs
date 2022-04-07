using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Structures
{
    class MyList<T> : IEnumerable<T>
    {
        protected ListElem<T> head = null;
        protected ListElem<T> tail = null;
        protected int length = 0;
        protected int capacity = 10;


        public int Length { get { return length; } }

        public void Erase()
        {
            ListElem<T> tmp;
            while (head != null)
            {
                tmp = head.next;
                head = tmp;
            }
            tail = null;
        }

        public bool IsFull()
        {
            if (length > capacity) return true;
            else return false;
        }

        public bool IsNull()
        {
            if (head == null) return true;
            else return false;
        }

        public void Print()
        {
            Console.WriteLine(ToString());
            Console.WriteLine(Length);
        }

        public override string ToString()
        {
            string str = "";
            ListElem<T> tmp = head;
            while (tmp != null)
            {
                str += tmp.data + " ";
                tmp = tmp.next;
            }
            return str;
        }

        public IEnumerator<T> GetEnumerator()
        {
            ListElem<T> temp = head;
            while (true)
            {
                if (temp != null)
                {
                    yield return temp.data;
                    temp = temp.next;
                }
                else
                {
                    temp = head;
                    yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this as IEnumerator;
        }

    }
}
