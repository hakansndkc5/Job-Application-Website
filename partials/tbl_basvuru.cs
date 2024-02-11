
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace stajdeneme.Models
{
    
    [MetadataType(typeof(tbl_basvurumetadata))]
    public partial class tbl_basvuru
    {
    }
    public class tbl_basvurumetadata
    {
        stajdenemeEntities3 db = new stajdenemeEntities3();
        public int basvuru_id { get; set; }

        [Required(ErrorMessage = "Bu alan dolduruması zorunludur.")]
        [Display(Name = "İsim")]
        [StringLength(30,ErrorMessage ="Bu alana 3-30 karakter değer girebilirsiniz.",MinimumLength=(3))]
        public string basvuru_ad { get; set; }

        [Display(Name = "Soyisim")]
        [Required(ErrorMessage = "Bu alan dolduruması zorunludur."), StringLength(30, ErrorMessage = "Bu alana 3-30 karakter  değer girebilirsiniz.", MinimumLength = (3))]
        public string basvuru_soyad { get; set; }
        public string basvuru_uyruk { get; set; }
        [Display(Name = "Adres")]
        [Required(ErrorMessage = "Bu alan dolduruması zorunludur.")]
        public string basvuru_adres { get; set; }
       
        public int basvuru_okul { get; set; }
        
        public int basvuru_fakulte { get; set; }
        
        public int basvuru_bolum { get; set; }
        
        public string basvuru_sınıf { get; set; }

        [Display(Name = "Not Ortalama")]
        //[RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$",ErrorMessage = "Please enter a Amount between 0.0 and 4.0")]
        [Range(0, 4.0, ErrorMessage = "lütfen  0.0 arası 4.0 sayı giriniz")]
        [RequiredIf("basvuru_notortalama", "basvuru_notortalama", ErrorMessage = "Bir tarih seçiniz.")]
        public string basvuru_notortalama { get; set; }

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "Bu alan dolduruması zorunludur.")]
        public string basvuru_tel { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Bu alan dolduruması zorunludur.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail geçersiz..")]
        public string basvuru_email { get; set; }
        public string basvuru_becerilersql { get; set; }
        public string basvuru_becerilerc_ { get; set; }
        public string basvuru_becerilerhtml { get; set; }
        public string basvuru_becerilercss { get; set; }
        public string basvuru_becerileraspnet { get; set; }
        public string basvuru_becerilermvc { get; set; }
        public string basvuru_becerilerjs { get; set; }
        public string basvuru_becerilerjquery { get; set; }
        public string basvuru_becerilerjava { get; set; }
        public string basvuru_becerilerjson { get; set; }
        public string basvuru_becerilerxml { get; set; }
        public string basvuru_hobiler { get; set; }

        [RequiredIf("basvuru_başlangıc", "basvuru_başlangıc", ErrorMessage = "Bir tarih seçiniz.")]
        [Display(Name = "Başlangıç tarihi")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public string basvuru_başlangıc { get; set; }
        [RequiredIf("basvuru_bitis", "basvuru_bitis",  ErrorMessage = "Bir tarih seçiniz.")]
        [Display(Name = "Bitiş tarihi")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public string basvuru_bitis { get; set; }
        public string basvuru_yabancıdil { get; set; }
        public string basvuru_saglık { get; set; }
        public string basvuru_referans { get; set; }
        public string basvuru_github { get; set; }
        public string basvuru_covid { get; set; }
        public string basvuru_nasılbuldunuz { get; set; }
        public string basvuru_bizinedensectiniz { get; set; }
        public string basvuru_diger { get; set; }
        public string basvuru_foto { get; set; }
        public string basvuru_cv { get; set; }


        //[Display(Name = "İl")]
        //[Required(ErrorMessage = "Bu alan seçilmesi zorunludur.")]
        //public Nullable<int> basvuru_il { get; set; }
        //[Display(Name = "İlce")]
        //[Required(ErrorMessage = "Bu alan seçilmesi zorunludur.")]
        //public Nullable<int> basvuru_ilce { get; set; }


    }
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private String PropertyName { get; set; }
        private Object DesiredValue { get; set; }
        private readonly RequiredAttribute _innerAttribute;

        public RequiredIfAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
            _innerAttribute = new RequiredAttribute();
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);

            if (dependentValue != null && dependentValue.ToString() == DesiredValue.ToString())
            {
                if (!_innerAttribute.IsValid(value))
                {
                    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "requiredif",
            };
            rule.ValidationParameters["dependentproperty"] = PropertyName;
            rule.ValidationParameters["desiredvalue"] = DesiredValue is bool ? DesiredValue.ToString().ToLower() : DesiredValue;

            yield return rule;
        }
    }

}