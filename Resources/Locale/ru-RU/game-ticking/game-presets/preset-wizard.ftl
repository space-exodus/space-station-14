## Survivor

roles-antag-survivor-name = Выживший
# It's a Halo reference
roles-antag-survivor-objective = Текущая задача: Выжить
survivor-role-greeting =
    Вы - выживший.
    Ваша главная задача остаться в живых и вернуться на Центком.
    Накопите столько огневой мощи, сколько необходимо для гарантии вашего выживания.
    Никому не доверяйте.
survivor-round-end-dead-count =
    { $deadCount ->
<<<<<<< HEAD
        [one] [color=red]{ $deadCount }[/color] выживший погиб.
       *[other] [color=red]{ $deadCount }[/color] выживших погибло.
    }
survivor-round-end-alive-count =
    { $aliveCount ->
        [one] [color=yellow]{ $aliveCount }[/color] выживший был оставлен на станции.
       *[other] [color=yellow]{ $aliveCount }[/color] выжившие были оставлены на станции.
    }
survivor-round-end-alive-on-shuttle-count =
    { $aliveCount ->
        [one] [color=green]{ $aliveCount }[/color] выживший смог выбраться живым.
       *[other] [color=green]{ $aliveCount }[/color] выжившие смогли выбраться живыми.
=======
        [one] [color=red]{ $deadCount }[/color] выживший умер.
       *[other] [color=red]{ $deadCount }[/color] выживших умерло.
    }
survivor-round-end-alive-count =
    { $aliveCount ->
        [one] [color=yellow]{ $aliveCount }[/color] выживший остался на станции.
       *[other] [color=yellow]{ $aliveCount }[/color] выживших осталось на станции.
    }
survivor-round-end-alive-on-shuttle-count =
    { $aliveCount ->
        [one] [color=green]{ $aliveCount }[/color] выживший выбрался живым.
       *[other] [color=green]{ $aliveCount }[/color] выживших выбралось живыми.
>>>>>>> space-syndicate/master
    }

## Wizard

<<<<<<< HEAD
objective-issuer-swf = [color=turquoise]Конфедерация Магов[/color]
wizard-title = Маг
wizard-description = На станции появился маг! Никогда не знаешь, что он может сделать.
roles-antag-wizard-name = Маг
roles-antag-wizard-objective = Преподать им урок, который они никогда не забудут.
wizard-role-greeting =
    ТЫ — МАГ!
    Между Конфедерацией Магов и NanoTrasen возникли трения.
    Поэтому Конфедерация выбрала вас, чтобы вы навестили станцию.
    Продемонстрируйте им всю мощь своих способностей.
    Что вы будете делать, зависит от вас, но помните: Конфедерация хочет, чтобы вы остались в живых.
wizard-round-end-name = маг

## TODO: Wizard Apprentice (Coming sometime post-wizard release)

=======
objective-issuer-swf = [color=turquoise]Федерация космических волшебников[/color]
wizard-title = Волшебник
wizard-description = На станции присутствует волшебник! Никогда не знаешь, что они могут натворить.
roles-antag-wizard-name = Волшебник
roles-antag-wizard-objective = Преподайте им урок, который они никогда не забудут.
wizard-role-greeting =
    ТЫ ВОЛШЕБНИК!
    Между Федерацией космических волшебников и Nanotrasen возникли противоречия.
    И вы были выбраны Федерацией Космических Волшебников, чтобы нанести на станцию визит.
    Хорошенько продемонстрируйте им свои способности.
    Вам решать, что именно предпринять, но помните, что Космические волшебники желают, чтобы вы вернулись живыми.
wizard-round-end-name = волшебник

## TODO: Wizard Apprentice (Coming sometime post-wizard release)

>>>>>>> space-syndicate/master
