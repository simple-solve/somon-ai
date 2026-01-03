import {useTranslations} from "next-intl";

const HomePage = () => {
  const t = useTranslations()
  return (
    <div>
      {t("HomePage.title")}
    </div>
  )
}

export default HomePage
