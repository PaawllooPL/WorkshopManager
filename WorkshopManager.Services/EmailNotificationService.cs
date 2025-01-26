using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Web.Models;
using WorkshopManager.ViewModels.VM;
using WorkshopManager.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace WorkshopManager.Services
{
    public class EmailNotificationService
    {
        private SmtpSettings _smtpSettings;
        private ApplicationDbContext _context;

        public EmailNotificationService(SmtpSettings smtpSettings, ApplicationDbContext context)
        {
            _smtpSettings = smtpSettings;
            _context = context; 
        }

        public async void SendEmail(string to, string title, string message)
        {
            using (var client = new SmtpClient(_smtpSettings.host, _smtpSettings.port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_smtpSettings.email, _smtpSettings.password);
                client.Timeout = 10000;

                client.Send(new MailMessage(_smtpSettings.email, to, title, message));
            }
        }

        public void SendRepairCostEstimatedNotification(int repairOrderId)
        {
            if (!_smtpSettings.isActive)
                return;

            var repairOrder = _context.RepairOrders
                                        .Include(ro => ro.Client)
                                        .FirstOrDefault(ro => ro.Id == repairOrderId);
            if (repairOrder == null)
                return;

            string title = "WorkshopManager, decyzja w sprawie zgłoszenia naprawy.";
            string message = $"Szanowny kliencie, naprawa w sprawie pojazdu o rejestracji {repairOrder.RegistrationNumber} dostała wycenę. " +
                $"Proszę o decyzję na platformie Workshop Manager.";

            SendEmail(repairOrder.Client.Email, title, message);
        }
        public void SendRepairRequestRejectNotification(int repairOrderId)
        {
            if (!_smtpSettings.isActive)
                return;

            var repairOrder = _context.RepairOrders
                                        .Include(ro => ro.Client)
                                        .FirstOrDefault(ro => ro.Id == repairOrderId);
            if (repairOrder == null)
                return;

            string title = "WorkshopManager, decyzja w sprawie zgłoszenia naprawy.";
            string message = $"Szanowny kliencie, naprawa w sprawie pojazdu o rejestracji {repairOrder.RegistrationNumber} została odrzucona.";

            SendEmail(repairOrder.Client.Email, title, message);
        }
        public void SendStartRepairNotification(int repairOrderId)
        {
            if (!_smtpSettings.isActive)
                return;

            var repairOrder = _context.RepairOrders
                                        .Include(ro => ro.Client)
                                        .FirstOrDefault(ro => ro.Id == repairOrderId);
            if (repairOrder == null)
                return;

            string title = "WorkshopManager, informacja o statusie naprawy.";
            string message = $"Szanowny kliencie, naprawa pojazdu o rejestracji {repairOrder.RegistrationNumber} właśnie się rozpoczęła.";
            SendEmail(repairOrder.Client.Email, title, message);
        }
        public void SendAddedRepairTaskNotification(int repairOrderId)
        {
            if (!_smtpSettings.isActive)
                return;

            var repairOrder = _context.RepairOrders
                                        .Include(ro => ro.Client)
                                        .FirstOrDefault(ro => ro.Id == repairOrderId);
            if (repairOrder == null)
                return;

            string title = "WorkshopManager, informacja o naprawie.";
            string message = $"Szanowny kliencie, do naprawy pojazdu o rejestracji {repairOrder.RegistrationNumber} dodano czynność naprawczą. " +
                $"Proszę o decyzję na platformie WorkShop Manager w oknie szczegółów naprawy.";
            SendEmail(repairOrder.Client.Email, title, message);
        }

        public void SendRepairCompletedNotification(int repairOrderId)
        {
            if (!_smtpSettings.isActive)
                return;

            var repairOrder = _context.RepairOrders
                                        .Include(ro => ro.Client)
                                        .FirstOrDefault(ro => ro.Id == repairOrderId);
            if (repairOrder == null)
                return;

            string title = "WorkshopManager, informacja o naprawie.";
            string message = $"Szanowny kliencie, naprawy pojazdu o rejestracji {repairOrder.RegistrationNumber} została zakończona. " +
                $"Pojazd jest gotowy do odbioru.";
            SendEmail(repairOrder.Client.Email, title, message);
        }

    }
}
