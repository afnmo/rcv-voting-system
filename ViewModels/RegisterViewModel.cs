using System.ComponentModel.DataAnnotations;

namespace VotingSystem.ViewModels;
public class RegisterViewModel
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public string Email { get; set; }

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(6, ErrorMessage = "كلمة المرور لا تقل عن 6 أحرف")]
    public string Password { get; set; }

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين")]
    public string ConfirmPassword { get; set; }
    
    public bool AcceptTerms { get; set; }


}