namespace Starikov.dbModels
open Unchecked
type ICafedraEntities =
    //abstract member GetValuesOfProperties : (string * obj) []
    //default this.GetValuesOfProperties = [| for pi in (this.GetType()).GetProperties() do yield (pi.Name, pi.GetValue(this)) |]
    abstract member GetNamesOfProperties : unit -> string []

type Person () =
    member val id_man = defaultof<int> with get, set
    member val fam = defaultof<string> with get, set
    member val name = defaultof<string> with get, set
    member val otchestvo = defaultof<string> with get, set
    member val INN = defaultof<string> with get, set
    member val sv_vo_PFR = defaultof<string> with get, set
    member val serial_pasport = defaultof<string> with get, set
    member val number_pasport = defaultof<string> with get, set
    member val data_vidachi_pasporta = defaultof<string> with get, set
    member val kem_vidan = defaultof<string> with get, set
    member val data_rojdeniya = defaultof<string> with get, set
    member val strana_1 = defaultof<string> with get, set
    member val region_1 = defaultof<string> with get, set
    member val index_1 = defaultof<string> with get, set
    member val gorod_1 = defaultof<string> with get, set
    member val ulica_1 = defaultof<string> with get, set
    member val dom_1 = defaultof<string> with get, set
    member val kv_1 = defaultof<string> with get, set
    member val strana_2 = defaultof<string> with get, set
    member val region_2 = defaultof<string> with get, set
    member val index_2 = defaultof<string> with get, set
    member val gorod_2 = defaultof<string> with get, set
    member val ulica_2 = defaultof<string> with get, set
    member val dom_2 = defaultof<string> with get, set
    member val kv_2 = defaultof<string> with get, set
    member val telefon_dom = defaultof<string> with get, set
    member val telefon_sot = defaultof<string> with get, set
    member val e_mail = defaultof<string> with get, set
    member val mesto_raboti = defaultof<string> with get, set
    member val ceh_otdel = defaultof<string> with get, set
    member val doljnost = defaultof<string> with get, set
    member val telefon_rabochii = defaultof<string> with get, set
    member val semeinoe_polojenie = defaultof<string> with get, set
    member val nomer_vb = defaultof<string> with get, set
    member val data_vidachi_vb = defaultof<string> with get, set
    member val kategoruya_godnosti = defaultof<string> with get, set
    member val komissariat = defaultof<string> with get, set
    member val kategoriya_zapasa = defaultof<string> with get, set
    member val voinskoe_zvanie = defaultof<string> with get, set
    member val kod_VUS = defaultof<string> with get, set
    member val sostav_profil = defaultof<string> with get, set
    member val voinskii_uchet = defaultof<string> with get, set
    member val chto_zakonchil = defaultof<string> with get, set
    member val kogda_zakonchil = defaultof<string> with get, set
    member val nacionalnost = defaultof<string> with get, set
    member val pol = defaultof<string> with get, set
    member val data_change = defaultof<string> with get, set
    member val dokument_ob_obrazov = defaultof<string> with get, set
    member val serial_dokumenta = defaultof<string> with get, set
    member val number_dokumenta = defaultof<string> with get, set
    member val data_reg_po_mj = defaultof<string> with get, set
    member val sem_polojenie_OKIN = defaultof<string> with get, set
    member val kod_OKATO = defaultof<string> with get, set
    member val kod_OKIN_grajd = defaultof<string> with get, set
    member val kod_OKIN_inyaz = defaultof<string> with get, set
    member val staj = defaultof<string> with get, set
    member val kod1 = defaultof<string> with get, set
    member val data_zapoln_anketi = defaultof<string> with get, set
    interface ICafedraEntities with
        //member this.GetValuesOfProperties = [| for pi in (this.GetType()).GetProperties() do yield (pi.Name, pi.GetValue(this)) |]
        member this.GetNamesOfProperties () = 
            [| "УИД"; "Фамилия"; "Имя"; "Отчество"; "ИНН"; "Св-во ПФР"; "Серия паспорта"; "Номер паспорта"; "Дата выдачи паспорта"; "Кем выдан"; "Дата рождения"; "Страна"; "Регион"; "Индекс"; "Город"; "Улица"; "Дом"; "Квартира"; "Страна по прописке"; "Регион по прописке"; "Индекс по прописке"; "Город по прописке"; "Улица по прописке"; "Дом по прописке"; "Квартира по прописке"; "Домашний телефон"; "Сотовый телефон"; "Адрес почты"; "Место работы"; "Цех/отдел"; "Должность"; "Рабочий телефон"; "Семейное положение"; "Номер военного билета"; "Дата выдачи военного билета"; "Категория годности"; "Комиссариат"; "Категория запаса"; "Воинское звание"; "Код ВУС"; "Состав/Профиль"; "Воинский учет"; "Что закончил"; "Когда закончил"; "Национальность"; "Пол"; "Дата изменения"; "Документ об образовании"; "Серия документа"; "Номер документа"; "Дата регистрации по месту жительства"; "Семейное положение ОКИН"; "Код ОКАТО"; "Код ОКИН гражданина"; "Код ОКИН иностранца"; "Стаж"; "Код \"1\""; "Дата заполнения анкеты" |]
type Student () =
    member val id_man = defaultof<int> with get, set
    member val number_zach = defaultof<string> with get, set
    member val id_group = defaultof<System.Nullable<int>> with get, set
    member val kod_platelshik = defaultof<System.Nullable<int>> with get, set
    member val god_postup = defaultof<string> with get, set
    member val otchislen_iz_group = defaultof<System.Nullable<int>> with get, set
    member val number_kontrakta = defaultof<string> with get, set
    member val kod1 = defaultof<string> with get, set
    member val data_change = defaultof<string> with get, set
    interface ICafedraEntities with
        //member this.GetValuesOfProperties = [| for pi in (this.GetType()).GetProperties() do yield (pi.Name, pi.GetValue(this)) |]
        member this.GetNamesOfProperties () = 
            [| "УИД Человека"; "Номер зачетки"; "УИД Группы"; "Код плательщика"; "Год поступления"; "Отчислен из группы"; "Номер контракта"; "Код \"1\""; "Дата изменения" |]