using Helpers.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Royalty.Additional
{
    public static class StaticCommands
    {
        private static DelegateCommand openUrlCommand = null;
        public static ICommand OpenUrlCommand { get { return openUrlCommand ?? (openUrlCommand = new DelegateCommand(o => {
            var url = o as string;
            if (url != null)
            {
                System.Diagnostics.Process.Start(url);
            }
        })); } }
    }
}
