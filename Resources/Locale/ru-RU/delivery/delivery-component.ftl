<<<<<<< HEAD
delivery-recipient-examine = Это предназначено для { $recipient }, { $job }.
delivery-already-opened-examine = Он уже открыт.
delivery-recipient-no-name = Безымянный
delivery-recipient-no-job = Неизвестный
delivery-unlocked-self = Вы разблокировали { $delivery } своим отпечатком.
delivery-opened-self = Вы открыли { $delivery }.
delivery-unlocked-others = { CAPITALIZE($recipient) } разблокировал { $delivery } с помощью { POSS-ADJ($possadj) } отпечатка.
delivery-opened-others = { CAPITALIZE($recipient) } открыл { $delivery }.
delivery-unlock-verb = Разблокировать
delivery-open-verb = Открыть
=======
delivery-recipient-examine = Адресовано: { $recipient }, { $job }.
delivery-already-opened-examine = Уже вскрыто.
delivery-recipient-no-name = Безымянный
delivery-recipient-no-job = Неизвестно
delivery-unlocked-self = Вы разблокировали { $delivery } отпечатком пальца.
delivery-opened-self = Вы вскрываете { $delivery }.
delivery-unlocked-others =
    { CAPITALIZE($recipient) } { GENDER($recipient) ->
        [male] разблокировал
        [female] разблокировала
        [epicene] разблокировали
       *[neuter] разблокировало
    } { $delivery } используя свой отпечаток пальца.
delivery-opened-others =
    { CAPITALIZE($recipient) } { GENDER($recipient) ->
        [male] вскрыл
        [female] вскрыл
        [epicene] вскрыл
       *[neuter] вскрыл
    } { $delivery }.
delivery-unlock-verb = Разблокировать
delivery-open-verb = Вскрыть
>>>>>>> space-syndicate/master
