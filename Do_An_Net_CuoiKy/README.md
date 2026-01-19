# 🐾 Shop Thú Cưng MVC - Đồ Án .NET

## 📋 TỔNG QUAN

Website bán hàng thú cưng được xây dựng bằng ASP.NET Core MVC.

---

## 📁 CẤU TRÚC THƯ MỤC

```
Do_An_Net_CuoiKy\                    ← Thư mục CHA (chứa solution)
│
├── Do_An_Net_CuoiKy\                ← Thư mục CON (PROJECT thực sự - chứa code)
│   ├── Controllers/                 # Controllers
│   ├── Models/                      # Database Models
│   ├── Views/                       # Razor Views
│   ├── Data/                        # DbContext
│   ├── wwwroot/                     # Static files (CSS, JS, images)
│   ├── Scripts/                     # SQL Scripts
│   ├── Program.cs                   # Entry point
│   └── Do_An_Net_CuoiKy.csproj     # Project file
│
└── Do_An_Net_CuoiKy.slnx           ← Solution file (Visual Studio)
```

**Lưu ý:** Project thực sự nằm trong **thư mục con** `Do_An_Net_CuoiKy\`

---

## 🚀 CÁCH CHẠY PROJECT

### Bước 1: Mở Terminal trong VS Code hoặc terminal của máy ae ( cmd ) 
- Nhấn `Ctrl + `` (dấu backtick)

### Bước 2: Di chuyển đến thư mục có chứa project mà anh em tải về 
```bash
cd C:\Do_An_Net\Do_An_Net_CuoiKy\Do_An_Net_CuoiKy
```

### Bước 3: Chạy project
```bash
anh em sau khi di chuyển đến chỗ chứa thư mục trong cmd thì gõ lệnh này để chạy nhé 
dotnet run
```

### Bước 4: Mở trình duyệt
chạy xong thì anh em truy cập 1 trong 2 địa chỉ này nhé, đây là địa chỉ web 
- `http://localhost:5016` hoặc `https://localhost:7253`

---


## ⚠️ LƯU Ý

- **Dừng server:** Nhấn `Ctrl + C` trong Terminal
- **Nếu gặp lỗi process đang chạy:** Mở Task Manager → End Task "Do_An_Net_CuoiKy.exe"
- **Database:** Hiện tại đã được comment trong `Program.cs` để test frontend không cần database
- **Khi mở project:** Mở **thư mục con** `Do_An_Net_CuoiKy\` trong VS Code (không phải thư mục cha)

---

## 🎯 TÍNH NĂNG

- ✅ Trang chủ với banner và sản phẩm nổi bật
- ✅ Danh sách sản phẩm với filter
- ✅ Chi tiết sản phẩm
- ✅ Giỏ hàng
- ✅ Thanh toán
- ✅ Đăng nhập/Đăng ký
- ✅ Responsive design

---

