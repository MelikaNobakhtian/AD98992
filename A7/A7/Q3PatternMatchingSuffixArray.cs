﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q3PatternMatchingSuffixArray : Processor
    {
        public Q3PatternMatchingSuffixArray(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, long, string[], long[]>)Solve, "\n");

        public HashSet<long> Searches;
        public long Count=0;
        protected virtual long[] Solve(string text, long n, string[] patterns)
        {
            Count = 0;
            Searches = new HashSet<long>();
            text = text + "$";
            var suffix = Solve(text);
            for (int i = 0; i < n; i++)
                SearchForPattern(patterns[i], text, text.Length, suffix);
            if (Count == 0)
                Searches.Add(-1);
            return Searches.ToArray();
        }

        public void SearchForPattern(string pattern,string text,long n,long[] suffixarray)
        {
            int left = 0;
            int right =(int) n;
            int mid = 0;
            while (left < right)
            {
                mid = (left + right) / 2;
                long end = Math.Min(suffixarray[mid] + pattern.Length, n );
                string sub = text.Substring((int)suffixarray[mid], (int)end - (int)suffixarray[mid]);
                if (pattern.CompareTo(sub) > 0)
                    left = mid+1;
                else
                    right = mid;
            }
            int start = left;
            right = (int)n;
            while (left < right)
            {
                mid = (left + right) / 2;
                long end = Math.Min(suffixarray[mid] + pattern.Length, n );
                string sub = text.Substring((int)suffixarray[mid], (int)end - (int)suffixarray[mid]);
                if (pattern.CompareTo(sub) < 0)
                    right = mid;
                else
                    left = mid + 1;
            }
            for (int i = start; i < right; i++)
            {
                Searches.Add(suffixarray[i]);
                Count++;
            }
        }

        public virtual long[] Solve(string text)
        {
            long[] order = SortCharacters(text);
            long[] classes = ComputeClasses(text, order);
            int l = 1;
            int len = text.Length;
            while (l < len)
            {
                order = Sortdoubled(len, order, classes, l);
                classes = UpdateClasses(order, classes, l);
                l = 2 * l;
            }
            return order;
        }

        public long[] UpdateClasses(long[] neworder, long[] classes, int l)
        {
            int n = neworder.Length;
            long[] newclass = new long[n];
            newclass[neworder[0]] = 0;
            for (int i = 1; i < n; i++)
            {
                long cur = neworder[i];
                long pre = neworder[i - 1];
                long mid = (cur + l) % n;
                long midpre = (pre + l) % n;
                if (classes[cur] != classes[pre] || classes[mid] != classes[midpre])
                    newclass[cur] = newclass[pre] + 1;
                else
                    newclass[cur] = newclass[pre];
            }
            return newclass;
        }
        public long[] Sortdoubled(int textlength, long[] orders, long[] classes, int l)
        {
            long[] neworder = new long[textlength];
            long[] count = new long[textlength];
            for (int i = 0; i < textlength; i++)
                count[classes[i]] += 1;
            for (int i = 1; i < textlength; i++)
                count[i] += count[i - 1];
            for (int i = textlength - 1; i >= 0; i--)
            {
                long start = (orders[i] - l + textlength) % textlength;
                long cl = classes[start];
                count[cl] -= 1;
                neworder[count[cl]] = start;
            }
            return neworder;
        }

        public long[] ComputeClasses(string text, long[] orders)
        {
            int len = text.Length;
            long[] classes = new long[len];
            classes[orders[0]] = 0;
            for (int i = 1; i < len; i++)
            {
                if (text[(int)orders[i]] != text[(int)orders[i - 1]])
                    classes[orders[i]] = classes[orders[i - 1]] + 1;
                else
                    classes[orders[i]] = classes[orders[i - 1]];
            }
            return classes;
        }

        public long[] SortCharacters(string text)
        {
            int len = text.Length;
            long[] order = new long[len];
            SortedDictionary<char, int> alphabet = new SortedDictionary<char, int>();
            foreach (var ch in text)
            {
                if (!alphabet.ContainsKey(ch))
                    alphabet.Add(ch, 1);
                else
                    alphabet[ch] += 1;
            }
            List<char> keys = new List<char>(alphabet.Keys);
            char firstkey = keys[0];
            int firstvalue = alphabet[firstkey];
            foreach (var l in keys)
                if (l == firstkey)
                    continue;
                else
                {
                    alphabet[l] += firstvalue;
                    firstkey = l;
                    firstvalue = alphabet[firstkey];
                }
            for (int i = len - 1; i >= 0; i--)
            {
                char c = text[i];
                alphabet[c] -= 1;
                order[alphabet[c]] = i;
            }
            return order;

        }
    }
}
