import fs from 'fs';
import imagemin from 'imagemin';
import imageminMozJpeg from 'imagemin-mozjpeg';
import imageminPngquant from 'imagemin-pngquant';
import imageminGifSicle from 'imagemin-gifsicle';

(async () => {
  const files = await imagemin(
    [
      '../areas/admin/content/**/*.{jpg,jpeg,png,gif}',
      '../areas/landing/images/**/*.{jpg,jpeg,png,gif}',
      '../areas/landing/frontend/images/**/*.{jpg,jpeg,png,gif}',
    ],
    {
      plugins: [
        imageminGifSicle({ interlaced: true }),
        imageminMozJpeg({ quality: 75, progressive: true }),
        imageminPngquant({ optimizationLevel: 5 }),
      ],
    },
  );

  files.forEach((filesItem) => {
    fs.writeFileSync(filesItem.sourcePath, filesItem.data);
  });

  console.log(`Optimize ${files.length} images`);
})();
