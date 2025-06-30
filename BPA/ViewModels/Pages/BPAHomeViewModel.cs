using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPA.ViewModels.Pages
{
    public partial class BPAHomeViewModel : ObservableObject
    {
        [ObservableProperty]
        string home = "你好啊";
    }
}
