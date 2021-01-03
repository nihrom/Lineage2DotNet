﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    //TODO: Рассмотреть возможность избавиться от этого класса
    public class BinaryNodeHeap
    {
        private Node[] _list;
        private int _size;

        public BinaryNodeHeap(int size)
        {
            _list = new Node[size + 1];
            _size = 0;
        }

        public void add(Node n)
        {
            _size++;
            int pos = _size;
            _list[pos] = n;
            while (pos != 1)
            {
                int p2 = pos / 2;
                if (_list[pos].getCost() <= _list[p2].getCost())
                {
                    Node temp = _list[p2];
                    _list[p2] = _list[pos];
                    _list[pos] = temp;
                    pos = p2;
                }
                else
                    break;
            }
        }

        public Node removeFirst()
        {
            Node first = _list[1];
            _list[1] = _list[_size];
            _list[_size] = null;
            _size--;
            int pos = 1;
            int cpos;
            int dblcpos;
            Node temp;
            while (true)
            {
                cpos = pos;
                dblcpos = cpos * 2;
                if ((dblcpos + 1) <= _size)
                {
                    if (_list[cpos].getCost() >= _list[dblcpos].getCost())
                        pos = dblcpos;
                    if (_list[pos].getCost() >= _list[dblcpos + 1].getCost())
                        pos = dblcpos + 1;
                }
                else if (dblcpos <= _size)
                {
                    if (_list[cpos].getCost() >= _list[dblcpos].getCost())
                        pos = dblcpos;
                }

                if (cpos != pos)
                {
                    temp = _list[cpos];
                    _list[cpos] = _list[pos];
                    _list[pos] = temp;
                }
                else
                    break;
            }
            return first;
        }

        public bool contains(Node n)
        {
            if (_size == 0)
                return false;
            for (int i = 1; i <= _size; i++)
            {
                if (_list[i].equals(n))
                    return true;
            }
            return false;
        }

        public bool isEmpty()
        {
            return _size == 0;
        }
    }
}
