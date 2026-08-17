using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Data;

public class SeedData
{
   
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category("Software and IT"),
                    new Category("Marketing and Advertising"),
                    new Category("Design and Art"),
                    new Category("Sales"),
                    new Category("Finance and Accounting"),
                    new Category("Customer Support")
                };
                await context.Categories.AddRangeAsync(categories);
            }

            if (!await context.Provinces.AnyAsync())
            {
                var tehran = new Province("Tehran");
                var isfahan = new Province("Isfahan");
                var fars = new Province("Fars");

                await context.Provinces.AddRangeAsync(tehran, isfahan, fars);

                var cities = new List<City>
                {
                    new City("Tehran", tehran.Id),
                    new City("Shemiranat", tehran.Id),
                    new City("Rey", tehran.Id),
                    new City("Isfahan", isfahan.Id),
                    new City("Kashan", isfahan.Id),
                    new City("Shiraz", fars.Id)
                };
                await context.Cities.AddRangeAsync(cities);
            }
            if (!await context.Skills.AnyAsync())
            {
                var skills = new List<Skill>
                {
                    new Skill("C# and .NET"),
                    new Skill("ASP.NET Core Web API"),
                    new Skill("SQL Server / Entity Framework"),
                    new Skill("JavaScript / TypeScript"),
                    new Skill("React.js"),
                    new Skill("Python"),
                    new Skill("Docker and DevOps"),
                    new Skill("Git and GitHub"),
                    new Skill("Project Management"),
                    new Skill("UI/UX Design")
                };
                await context.Skills.AddRangeAsync(skills);
            }
            if (!await context.EmailTemplates.AnyAsync())
            {
                var templates = new List<EmailTemplate>
                {
                    // ۱. قالب تایید کارفرما
                    new EmailTemplate(
                        EmailEventTypeEnum.EmployerApproved,
                        "تایید حساب کاربری کارفرما - HireFlow",
                        "سلام {Name} عزیز،\n\nبا خوشحالی به اطلاع می‌رسانیم که حساب کاربری شما برای شرکت «{CompanyName}» توسط مدیر سیستم تایید شد.\nاکنون می‌توانید وارد پنل خود شده و آگهی‌های استخدامی جدید ثبت کنید.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    ),

                    // ۲. قالب رد صلاحیت کارفرما
                    new EmailTemplate(
                        EmailEventTypeEnum.EmployerDisapproved,
                        "وضعیت ثبت‌نام حساب کاربری - HireFlow",
                        "سلام {Name} عزیز،\n\nمتاسفانه حساب کاربری کارفرمایی شما برای شرکت «{CompanyName}» در حال حاضر مورد تایید قرار نگرفت.\nجهت کسب اطلاعات بیشتر یا رفع ابهامات، لطفا با پشتیبانی سیستم در ارتباط باشید.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    ),

                    // ۳. تغییر وضعیت درخواست کارجو به در حال بررسی
                    new EmailTemplate(
                        EmailEventTypeEnum.RequestUnderReview,
                        "به‌روزرسانی وضعیت درخواست همکاری - {JobTitle}",
                        "سلام {Name} عزیز،\n\nدرخواست همکاری شما برای فرصت شغلی «{JobTitle}» در شرکت «{CompanyName}» به وضعیت «در حال بررسی» تغییر یافت.\nتغییرات بعدی از طریق ایمیل به اطلاع شما خواهد رسید.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    ),

                    // ۴. دعوت کارجو به مصاحبه
                    new EmailTemplate(
                        EmailEventTypeEnum.RequestInterview,
                        "دعوت به مصاحبه شغلی - {CompanyName}",
                        "سلام {Name} عزیز،\n\nخبر خوب! شرکت «{CompanyName}» رزومه شما را برای فرصت شغلی «{JobTitle}» بررسی کرده و تمایل دارد شما را به یک مصاحبه کاری دعوت کند.\nکارشناسان شرکت به زودی جهت هماهنگی زمان مصاحبه با شما تماس خواهند گرفت.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    ),

                    // ۵. پذیرش نهایی کارجو
                    new EmailTemplate(
                        EmailEventTypeEnum.RequestAccepted,
                        "تبریک! درخواست همکاری شما پذیرفته شد - {CompanyName}",
                        "سلام {Name} عزیز،\n\nبا کمال مسرت به اطلاع می‌رسانیم که درخواست همکاری شما برای موقعیت شغلی «{JobTitle}» در شرکت «{CompanyName}» پذیرفته شده است.\nمسئولین شرکت به زودی جهت انجام هماهنگی‌های بعدی و شروع همکاری با شما ارتباط برقرار خواهند کرد.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    ),

                    // ۶. رد درخواست کارجو
                    new EmailTemplate(
                        EmailEventTypeEnum.RequestRejected,
                        "به‌روزرسانی وضعیت درخواست همکاری - {JobTitle}",
                        "سلام {Name} عزیز،\n\nضمن تشکر از زمان و تلاشی که برای ارسال رزومه جهت موقعیت شغلی «{JobTitle}» در شرکت «{CompanyName}» گذاشتید، به اطلاع می‌رسانیم که پس از بررسی دقیق رزومه‌ها، متاسفانه در این مرحله امکان پیش رفتن با شما میسر نگردید.\nاطلاعات شما در بانک رزومه‌های ما محفوظ خواهد ماند تا در فرصت‌های آتی بررسی گردد. برای شما در ادامه‌ی مسیر حرفه‌ای آرزوی موفقیت داریم.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    ),

                    // ۷. ثبت درخواست جدید برای کارفرما
                    new EmailTemplate(
                        EmailEventTypeEnum.NewApplicationReceived,
                        "دریافت درخواست همکاری جدید برای آگهی «{JobTitle}»",
                        "کارفرمای محترم شرکت «{CompanyName}»،\n\nیک درخواست همکاری جدید از طرف کارجو «{Name}» برای فرصت شغلی «{JobTitle}» دریافت کردید.\nلطفاً جهت بررسی رزومه و اطلاعات این کارجو به پنل کاربری خود مراجعه کنید.\n\nبا احترام،\nتیم پشتیبانی HireFlow"
                    )
                };

                await context.EmailTemplates.AddRangeAsync(templates);
            }


            await context.SaveChangesAsync();
        }
    }
