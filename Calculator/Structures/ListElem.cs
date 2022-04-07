using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Structures
{
    class ListElem<E>
    {
        public ListElem<E> next;
        public ListElem<E> prev;
        public E data;

        public ListElem(E data)
        {
            this.data = data;
        }
    }
}
