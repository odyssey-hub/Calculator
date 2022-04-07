using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Structures
{
    class MyStack<T> : MyList<T>
    {
        public MyStack()
        {
            length = 100;
        }

        public MyStack(int capacity)
        {
            this.capacity = capacity;
        }

        public MyStack(IEnumerable collection)
        {
            IEnumerator enumerator = collection.GetEnumerator();
            capacity = 0;
            while (enumerator.MoveNext())
            {
                T data = (T)enumerator.Current;
                capacity++;
                Push(data);
            }
        }

        public bool Push(T data)
        {
            ListElem<T> el = new ListElem<T>(data);
            if (!IsNull()) el.next = head;
            else tail = el;
            head = el;
            length++;
            return true;
        }

        public T Peek()
        {
            return head.data;
        }

        public T Pop()
        {
            if (IsNull()) return default(T);
            ListElem<T> el = head;
            head = head.next;
            length--;
            return el.data;
        }


        public override bool Equals(object stack)
        {
            if (stack is MyStack<T>)
            {
                if (ToString() == ((MyStack<T>)stack).ToString()) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(MyStack<T> stack1, MyStack<T> stack2)
        {
            return stack1.Equals(stack2);
        }

        public static bool operator !=(MyStack<T> stack1, MyStack<T> stack2)
        {
            return !stack1.Equals(stack2);
        }
    }


}
