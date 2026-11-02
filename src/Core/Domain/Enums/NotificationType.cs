namespace Domain.Enums
{
    public enum NotificationType
    { 
        PaymentOverdue = 1,
        MaintenanceEscalation = 2,     // مخصص لتصعيد تذاكر الصيانة المتأخرة (SLA)
        ContractRenewalReminder = 3,   // مخصص لاقتراب موعد انتهاء العقد
        ContractOverstayAlert = 4    // مخصص لحالة تجاوز مدة العقد الفعلية
    }
}