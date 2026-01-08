const defaultTheme = require("tailwindcss/defaultTheme");

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{html,cshtml}",
    "./Pages/**/*.{html,cshtml}",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        transparent: 'transparent',
        'primary': '#025471',
        'primary-70': '#207FA0B2',
        'primary-80': '#0A6D90',
        'secondary': '#E2B124',
        'secondary-70': '#9ca641',
        'custom-mm': '#F3F3F31A',
        'primary-20': '#CBCBCB80',
        'ap': '#e8e8e8',
        'dash-card-1': '#E2B1244D',
        'dash-card-2': '#0AA1DD4D',
        'dash-card-3': '#2FA0394D',
        'dash-card-4': '#5A3E9E4D',
        'ap-grey-5': '#fbfaf0',
        'success': '#2FA039',
        'danger': '#EA1313'
      },
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
        mk: ["MontserratMedium", 'KHKulen'],
        mkBold: ["MontserratBold", 'KHKulen'],
        khmuol: ["MontserratMedium", 'KHMoul'],
        mm: ["MontserratMedium", 'khbattambong'],
        mn: ['MontserratN', 'KHSR'],
        mc: ['MontserratN', 'KHMetal'],
        kc: ["MontserratMedium", 'khcontent'],
        kb: ["MontserratMedium", 'khbattambong'],
        kbk: ["MontserratMedium", 'khbokor'],
      },
    }
  },
  plugins: []
};
