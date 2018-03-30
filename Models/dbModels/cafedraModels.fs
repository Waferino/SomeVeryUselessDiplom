namespace Starikov.dbModels
open Unchecked
type ICafedraEntities =
    //abstract member GetValuesOfProperties : (string * obj) []
    //default this.GetValuesOfProperties = [| for pi in (this.GetType()).GetProperties() do yield (pi.Name, pi.GetValue(this)) |]
    abstract member GetNamesOfProperties : unit -> string []
type People () =
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
            [| "УИД Человека"; "Номер зачетки"; "Группа"; "Код плательщика"; "Год поступления"; "Отчислен из группы"; "Номер контракта"; "Код \"1\""; "Дата изменения" |]
type Group () =
    member val id_group = defaultof<int> with get, set
    member val year_postup = defaultof<string> with get, set
    member val name_group = defaultof<string> with get, set
    member val form = defaultof<string> with get, set
    member val kurator = defaultof<string> with get, set
    member val starosta = defaultof<string> with get, set
    member val kolvo_studentov = defaultof<System.Nullable<int>> with get, set
    member val id_specialistic = defaultof<System.Nullable<int>> with get, set
    member val vipusk = defaultof<System.Nullable<int>> with get, set
    member val kod1 = defaultof<string> with get, set
    member val data_change = defaultof<string> with get, set
    member val year_okonchan = defaultof<string> with get, set
    interface ICafedraEntities with
        member this.GetNamesOfProperties () =
            [| "УИД Группы"; "Год поступления"; "Название группы"; "Форма"; "FK Куратор"; "FK Староста"; "Количество студентов"; "FK специальность"; "Выпуск"; "Код \"1\""; "Дата изменения"; "Год окончания"; |]
type Account () =
    member val id_man = defaultof<int> with get, set
    member val Email = defaultof<string> with get, set
    member val Password = defaultof<string> with get, set
    member val Role = defaultof<string> with get, set
    member val date_of_change = defaultof<string> with get, set
type EventInfo () =
    member val id_EventInfo = defaultof<int> with get, set
    member val DateOfThe = defaultof<System.Nullable<System.DateTime>> with get, set
    member val Name = defaultof<string> with get, set
    member val Notation = defaultof<string> with get, set //System.Nullable<>
    member val ChangedBy = defaultof<int> with get, set
    interface ICafedraEntities with
        member this.GetNamesOfProperties () =
            [| "УИД События"; "Ожидаемая дата проведения"; "Название события"; "Примачание к событию" |]
type Event () =
    member val id_Event = defaultof<int> with get, set
    member val id_EventInfo = defaultof<int> with get, set
    member val isGroup_Event = defaultof<bool> with get, set
    member val fk_student_or_group = defaultof<int> with get, set
    member val event_result = defaultof<string> with get, set
    member val creatingDate = defaultof<string> with get, set
type ExtraEvent () =
    member val id_ExtraEvent = defaultof<int> with get, set
    member val id_Event = defaultof<int> with get, set
    member val fileName = defaultof<string> with get, set
    member val contentType = defaultof<string> with get, set
    member val fileDataPath = defaultof<string> with get, set
    member val creatingDate = defaultof<string> with get, set
type AnceteInfo () =
    member val id_man = defaultof<int> with get, set
    member val childrens = defaultof<string> with get, set
    member val pasport_code = defaultof<string> with get, set
    member val district_1 = defaultof<string> with get, set
    member val district_2 = defaultof<string> with get, set
    member val id_language = defaultof<int> with get, set //i'll create dict with languages
    member val benefits = defaultof<string> with get, set
    member val educ_type = defaultof<string> with get, set //
    member val kontract_startday = defaultof<string> with get, set
    member val who_pays_kontract = defaultof<int> with get, set
    member val pastSport = defaultof<string> with get, set
    member val presentSport = defaultof<string> with get, set
    member val futureSport = defaultof<string> with get, set
    member val student_mother = defaultof<int> with get, set
    member val student_father = defaultof<int> with get, set
type LanguageDictionary () =
    member val id_language = defaultof<int> with get, set
    member val langName = defaultof<string> with get, set