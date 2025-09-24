import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';
import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);

import '../../../Content/src/settingsTemplatesDocx/settingsTemplatesDocx.js';
import '../../../Content/src/settingsTemplatesDocx/modal/addEditTemplate/addEditTemplate.js';
import '../../../Content/src/settingsTemplatesDocx/modal/DescriptionTemplate/descriptionTemplate.js';
appDependency.addItem('settingsTemplatesDocx');
