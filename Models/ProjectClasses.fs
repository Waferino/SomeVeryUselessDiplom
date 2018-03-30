namespace Starikov

type Messager (email: string, password: string) =
    member val fromEmail = email with get, set
    member val fromPassword = password with get, set
    interface IMessager with
        member this.SendMessage message_subject message_body sendTo =
            MessagerModule.Send (this.fromEmail, this.fromPassword) message_subject message_body sendTo
type AccountRegistrationHelper (akw: string, ckw: string) =
    member val AdminKeyWord = akw with get, set
    member val CuratorKeyWord = ckw with get, set
    interface IAccountRegistrationHelper with
        member this.GetCuratorsKeyWord = this.CuratorKeyWord
        member this.GetAdminsKeyWord = this.AdminKeyWord