//https://codewithmukesh.com/blog/integrating-tailwind-css-with-blazor/
//https://tailwindcss.com/docs/optimizing-for-production
/** @type {import('tailwindcss').Config} **/
module.exports = {
    theme: {
        extend: {}
    },
    content: [
        '../**/*.html',
        '../**/*.razor',
        '../**/*.razor.cs'
    ],
    plugins: [
        require('@tailwindcss/forms')
    ]
}
