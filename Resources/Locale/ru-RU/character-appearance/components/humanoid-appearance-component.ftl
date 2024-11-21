humanoid-appearance-component-unknown-species = гуманоид
humanoid-appearance-component-examine = { CAPITALIZE(SUBJECT($user)) } { $species } { $age }.
humanoid-appearance-component-mindset-examine = Мировозрение: { $mindset ->
  [orderlykind] Порядочно-добрый
  *[ordered] Порядочный
  [orderlyangry] Порядочно-злой
  [good] Добрый
  [trulyneutral] Истинно нейтральный
  [evil] Злой
  [chaoticallykind] Хаотично-добрый
  [chaotic] Хаотичный
  [chaoticallyangry] Хаотично-злой
}
