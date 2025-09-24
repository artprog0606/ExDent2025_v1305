import Swal from 'sweetalert2';

const Sweetalert2 = Swal.mixin({
    cancelButtonText: 'Отмена',
    confirmButtonText: 'ОK',
    allowOutsideClick: false,
    buttonsStyling: false,
    confirmButtonClass: 'btn btn-sm btn-success',
    cancelButtonClass: 'btn btn-sm btn-action',
    useRejections: true,
});

export default Sweetalert2;
