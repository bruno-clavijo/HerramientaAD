using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.utilerias
{
    public class Util
    {
        public class ItemCombo {
            private string text;
            private int value;

            public string Text { get => text; set => text = value; }
            public int Value { get => value; set => this.value = value; }
            public ItemCombo(string stext, int ivalue) {
                text = stext;
                value = ivalue;
            }
        }
    }
}