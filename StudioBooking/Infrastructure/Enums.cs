using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Infrastructure
{
    public static class Enums
    {
        public enum Roles
        {
            SuperAdmin,
            Admin,
            Moderator,
            Basic
        }

        public enum AdjustmentType
        {
            Add = 1,
            Deduct
        }

        public enum ServiceType
        {
            Basic,
            Calculated
        }

        public enum DetailType
        {
            LiveRoom,
            ControlRoom
        }

        public enum GearContentType
        {
            Basic,
            Icon,
            Image
        }

        public enum PaymentStatus
        {
            Pending,
            Advance,
            FullPayment,
            RefundPending,
            RefundCompleted
        }

        public enum BookingStatus
        {
            WaitingForApproval,
            Pending,
            OnHold,
            Booked,
            ReScheduled,
            WaitingForCancellation,
            Cancelled,
            Failed,
            Reserved
        }

        public enum PaymentMode
        {
            Offline = 1,
            Wallet = 2,
            Online = 4
        }

        public enum TransactionType
        {
            Debit,
            Credit
        }

        public enum PaymentType
        {
            Pending,
            Advance,
            Full,
            Refund,
            OnDemand
        }

        public enum TransactionStatus
        {
            Pending,
            Success,
            Failed
        }
        public enum PaymentProvider
        {
            Cash,
            UPI,
            BankTransfer,
            Wallet,
            CCAvenue,
            CashFree,
            Other
        }

        public enum WalletType
        {
            None,
            Points,
            Hours
        }

        public enum Professions
        {
            Owner = 1,
            Recording_Engineer,
            Partner,
            Accounts,
            Engineer,
            Studio_Team
        }

        public enum RequestType
        {
            ReSchedule = 1,
            Cancel,
            OnHold
        }

        public enum RequestStatus
        {
            Pending = 0,
            Completed,
            Rejected
        }
    }
}
