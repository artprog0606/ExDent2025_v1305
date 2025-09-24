import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/files/files.js';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';
import 'ng-file-upload';

appDependency.addItem(`swipeLine`);
appDependency.addItem(`files`);
