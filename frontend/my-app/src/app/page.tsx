import {useTranslations} from "next-intl";
import LanguageSwitcher from "@/src/components/ui/LanguageSwitcher/LanguageSwitcher";
import {Button} from "@mantine/core";

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
