using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Lib.Alg
{
    public class IndexedPriorityQueueLow<KeyType> 
        where KeyType : IComparable<KeyType>
    {
        private List<KeyType> VecKeys;
        private List<int> Heap;
        private List<int> InvHeap;

        private int Size;
        private int MaxSize;

        private void Swap(int a, int b)
        {
            int temp = Heap[a]; Heap[a] = Heap[b]; Heap[b] = temp;
            InvHeap[Heap[a]] = a; InvHeap[Heap[b]] = b;
        }

        private void ReorderUpdwards(int Node)
        {
            while ((Node > 1) && (VecKeys[Heap[Node/2]].CompareTo(VecKeys[Heap[Node]]) > 0))
            {
                Swap(Node / 2, Node);
                Node /= 2;
            }
        }

        private void ReorderDownwards(int Node, int HeapSize)
        {
            while (2 * Node <= HeapSize)
            {
                int Child = 2 * Node;
                if ((Child < HeapSize) && (VecKeys[Heap[Child]].CompareTo(VecKeys[Heap[Child+1]]) > 0))
                {
                    ++Child;
                }

                if (VecKeys[Heap[Node]].CompareTo(VecKeys[Heap[Child]]) > 0)
                {
                    Swap(Child, Node);
                    Node = Child;
                }
                else
                {
                    break;
                }
            }
        }

        public bool IsEmpty()
        {
            return Size == 0;
        }

        public IndexedPriorityQueueLow(List<KeyType> Keys, int MaxSize)
        {
            this.VecKeys = Keys;
            this.MaxSize = MaxSize;
            this.Size = 0;

            Heap = new List<int>(new int[MaxSize + 1]);
            InvHeap = new List<int>(new int[MaxSize + 1]);

            //Heap.Capacity = MaxSize + 1;
            //InvHeap.Capacity = MaxSize + 1;
        }

        // add to the end of heap, then reorder upwards
        public void Insert(int Index)
        {
            ++Size;
            Heap[Size] = Index;
            InvHeap[Index] = Size;
            ReorderUpdwards(Size);
        }

        public int Pop()
        {
            Swap(1, Size);
            ReorderDownwards(1, Size - 1);
            return Heap[Size--]; 
        }

        public void ChangePriority(int Index)
        {
            ReorderUpdwards(Heap[Index]);
        }
    }
}
