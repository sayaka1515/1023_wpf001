using Microsoft.Win32;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf001
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 定義飲料品項及其價格
        Dictionary<string, int> drinks = new Dictionary<string, int>
        {

        };

        // 定義訂單內容
        Dictionary<string, int> orders = new Dictionary<string, int>();
        // 定義外帶選項
        string takeout = "";

        public MainWindow()
        {
            InitializeComponent();

            // 讀取飲料品項檔案
            AddNewDrink(drinks);

            // 顯示飲料品項
            DisplayDrinkMenu(drinks);
        }

        // 顯示飲料品項的方法

        private void AddNewDrink(Dictionary<string, int> drinks)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // 建立 OpenFileDialog 物件
            openFileDialog.Title = "選擇飲料品項檔案"; // 設定對話框標題
            openFileDialog.Filter = "CSV文件 |*.csv|文字檔案|*.txt|所有檔案 |*.*"; // 設定檔案過濾器

            if (openFileDialog.ShowDialog() == true) // 顯示開啟檔案對話框，並檢查使用者是否選擇了檔案
            {
                string filename = openFileDialog.FileName; // 取得使用者選擇的檔案名稱
                string[] lines = File.ReadAllLines(filename); // 讀取檔案中的所有行

                foreach (var line in lines) // 逐行處理檔案內容
                {
                    string[] tokens = line.Split(','); // 將每行內容以逗號分隔
                    string drinkName = tokens[0]; // 取得飲料名稱
                    int price = Convert.ToInt32(tokens[1]); // 取得飲料價格並轉換為整數
                    drinks.Add(drinkName, price); // 將飲料名稱和價格添加到 drinks 字典中
                }
            }
        }

        private void DisplayDrinkMenu(Dictionary<string, int> drinks)
        {
            // 設定 stackpanel_DrinkMenu 的高度
            stackpanel_DrinkMenu.Height = 42 * drinks.Count;
            foreach (var drink in drinks)
            {
                // 建立 StackPanel
                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(3),
                    Background = Brushes.LightBlue,
                    Height = 35,
                };

                // 建立 CheckBox
                var cb = new CheckBox
                {
                    Content = drink.Key,
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Blue,
                    Width = 150,
                    Margin = new Thickness(5),
                    VerticalContentAlignment = VerticalAlignment.Center,
                };

                // 建立 Label 顯示價格
                var lb_price = new Label
                {
                    Content = $"{drink.Value}元",
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Green,
                    Width = 60,
                    VerticalContentAlignment = VerticalAlignment.Center,
                };

                // 建立 Slider
                var sl = new Slider
                {
                    Width = 150,
                    Minimum = 0,
                    Maximum = 10,
                    Value = 0,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    IsSnapToTickEnabled = true,
                };

                // 建立 Label 顯示數量
                var lb_amount = new Label
                {
                    Content = "0",
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 50,
                };

                // 綁定 Slider 的值到 Label
                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb_amount.SetBinding(ContentProperty, myBinding);

                // 將控制項添加到 StackPanel
                sp.Children.Add(cb);
                sp.Children.Add(lb_price);
                sp.Children.Add(sl);
                sp.Children.Add(lb_amount);

                // 將 StackPanel 添加到 stackpanel_DrinkMenu
                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        // 處理 RadioButton 的 Checked 事件
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                // 設定外帶選項
                takeout = rb.Content.ToString();
            }
        }

        // 處理訂單按鈕的點擊事件
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            // 確認訂購內容
            orders.Clear();
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var drinkName = cb.Content.ToString();
                var sl = sp.Children[2] as Slider;
                var amount = (int)sl.Value;

                // 如果選中且數量大於0，添加到訂單
                if (cb.IsChecked == true && amount > 0) orders.Add(drinkName, amount);
            }

            // 顯示訂購內容
            string msg = "";
            string discount_msg;
            int total = 0;

            DateTime datetime = DateTime.Now;

            msg += $"訂購時間：{datetime.ToString("yyyy-MM-dd HH:mm:ss")}，此次訂購為{takeout}，訂購內容如下：\n\n";
            
            int num = 1;
            foreach (var order in orders)
            {
                int subtotal = drinks[order.Key] * order.Value;
                msg += $"{num}. {order.Key} x {order.Value}杯，小計{subtotal}元\n";
                total += subtotal;
                num++;
            }
            msg += $"總金額：{total}元\n";
            int sellPrice = total;
            if (total >= 500)
            {
                sellPrice = (int)(total * 0.8);
                discount_msg = $"恭喜您獲得滿500元打8折優惠，折扣後售價為 {sellPrice}元：";
            }
            else if (total >= 300)
            {
                sellPrice = (int)(total * 0.9);
                discount_msg = $"恭喜您獲得滿300元打9折優惠，折扣後售價為 {sellPrice}元：";
            }
            else
            {
                discount_msg = $"未達到任何優惠條件，售價為{sellPrice}元";
            }
            msg += $"\n{discount_msg}，原價為{total}元，售價為 {sellPrice}元。";

            // 顯示結果
            ResultTextBlock.Text = msg;

            SaveFileDialog saveFileDialog = new SaveFileDialog(); // 建立 SaveFileDialog 物件
            saveFileDialog.Title = "儲存訂購內容"; // 設定對話框標題
            saveFileDialog.Filter = "文字檔案|*.txt|所有文件|*.*"; // 設定檔案過濾器

            if (saveFileDialog.ShowDialog() == true) // 顯示儲存檔案對話框，並檢查使用者是否選擇了檔案
            {
                string filename = saveFileDialog.FileName; // 取得使用者選擇的檔案名稱
                try
                {
                    // 使用 StreamWriter 物件寫入檔案
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        sw.Write(msg); // 將訂購內容寫入檔案
                    }
                    MessageBox.Show("訂購內容已儲存。"); // 顯示成功訊息
                }
                catch (Exception ex)
                {
                    // 捕捉例外並顯示錯誤訊息
                    MessageBox.Show($"儲存失敗: {ex.Message}");
                }
            }

        }
    }
}
