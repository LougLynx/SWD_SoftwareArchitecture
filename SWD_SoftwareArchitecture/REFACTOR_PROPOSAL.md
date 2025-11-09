# Đề Xuất Refactor: Chỉ Dùng Singleton + SPL Architecture

## 🎯 Mục Tiêu
Loại bỏ Strategy và Factory patterns, chỉ giữ lại:
- **Singleton Pattern**: FeatureManager
- **SPL Architecture**: Feature-based conditional logic

## 📋 Cách Thực Hiện

### 1. Giữ Nguyên Singleton Pattern
```csharp
// Core/ServiceRegistrationExtensions.cs
services.AddSingleton<FeatureManager>(); // ✅ Giữ nguyên
```

### 2. Loại Bỏ Strategy & Factory Patterns
- ❌ Xóa `IEnrollmentStrategy`, `IGradingStrategy`
- ❌ Xóa `EnrollmentStrategyFactory`, `GradingStrategyFactory`
- ❌ Xóa `StandardEnrollmentStrategy`, `StandardGradingStrategy`

### 3. Thay Thế Bằng Conditional Logic Trong Service

**Thay vì:**
```csharp
// ❌ Cũ: Dùng Strategy + Factory
var factory = _enrollmentStrategyFactory;
var strategy = factory.GetStrategy("Standard");
var result = await strategy.ProcessEnrollmentAsync(enrollmentDto);
```

**Thành:**
```csharp
// ✅ Mới: Dùng FeatureManager (Singleton) + if/switch
public async Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto enrollmentDto)
{
    // SPL Variability Point: Chọn logic dựa trên feature flags
    var productVariant = _featureManager.GetProductVariant(); // "Standard", "Premium", etc.
    
    switch (productVariant)
    {
        case "Premium":
            // Premium enrollment logic
            return await CreatePremiumEnrollmentAsync(enrollmentDto);
        case "Standard":
        default:
            // Standard enrollment logic
            return await CreateStandardEnrollmentAsync(enrollmentDto);
    }
}
```

## 🔄 Code Mẫu Refactored

### EnrollmentService (Simplified)
```csharp
public class EnrollmentService : BaseService, IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly FeatureManager _featureManager; // ✅ Inject Singleton

    public EnrollmentService(
        ILogger<EnrollmentService> logger,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        FeatureManager featureManager) // ✅ Inject Singleton
        : base(logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _featureManager = featureManager; // ✅ SPL Architecture
    }

    public async Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto enrollmentDto)
    {
        // SPL Variability Point: Chọn logic dựa trên product variant
        var productVariant = _featureManager.GetProductVariant();
        
        // Conditional logic thay vì Strategy Pattern
        if (productVariant == "Premium" && _featureManager.IsEnabled("PremiumEnrollment"))
        {
            return await CreatePremiumEnrollmentAsync(enrollmentDto);
        }
        
        // Standard enrollment (default)
        return await CreateStandardEnrollmentAsync(enrollmentDto);
    }

    private async Task<ServiceResult<EnrollmentDto>> CreateStandardEnrollmentAsync(EnrollmentDto enrollmentDto)
    {
        // Standard enrollment logic
        // ... existing code ...
    }

    private async Task<ServiceResult<EnrollmentDto>> CreatePremiumEnrollmentAsync(EnrollmentDto enrollmentDto)
    {
        // Premium enrollment logic
        // Auto-assign mentor, send welcome package, etc.
        // ... premium-specific code ...
    }
}
```

### ServiceRegistrationExtensions (Simplified)
```csharp
public static IServiceCollection AddFeatureServices(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // ✅ Lấy FeatureManager từ DI (Singleton)
    var featureManager = services.BuildServiceProvider()
        .GetRequiredService<FeatureManager>();

    // Core Services
    services.AddScoped<IEnrollmentService, EnrollmentService>();
    services.AddScoped<IGradingService, GradingService>();

    // ❌ Xóa Strategy & Factory registration
    // services.AddScoped<IEnrollmentStrategy, StandardEnrollmentStrategy>();
    // services.AddScoped<EnrollmentStrategyFactory>();

    // ✅ Conditional feature registration (SPL)
    if (featureManager.IsEnabled(FeatureFlags.AdvancedReporting))
    {
        // services.AddScoped<IAdvancedReportingService, AdvancedReportingService>();
    }

    return services;
}
```

## ✅ Lợi Ích

1. **Đơn giản hơn**: Ít class, ít abstraction
2. **Dễ hiểu**: Logic rõ ràng trong Service
3. **Vẫn giữ SPL**: Feature flags + conditional logic
4. **Singleton**: FeatureManager vẫn là Singleton

## ⚠️ Trade-offs

### Mất đi:
- ❌ Strategy Pattern flexibility
- ❌ Factory Pattern encapsulation
- ❌ Dễ mở rộng variants (phải sửa Service code)

### Được:
- ✅ Code đơn giản hơn
- ✅ Dễ debug
- ✅ Vẫn đạt SPL architecture
- ✅ Singleton pattern được giữ nguyên

## 🎯 Kết Luận

**Có thể** chỉ dùng Singleton + SPL architecture bằng cách:
1. Giữ `FeatureManager` (Singleton)
2. Dùng conditional logic (if/switch) trong Services
3. Loại bỏ Strategy và Factory patterns

**Nhưng** sẽ mất tính linh hoạt và khả năng mở rộng của Strategy Pattern.

