import {useTranslations} from "next-intl";
import LanguageSwitcher from "@/src/components/ui/LanguageSwitcher/LanguageSwitcher";

const HomePage = () => {
  const t = useTranslations()
  return (
    <div>
      <LanguageSwitcher />
      {t("HomePage.title")}
    </div>
  )
}

export default HomePage
