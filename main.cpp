#include <iostream>
#include <link.h>
#include <fstream>

using namespace std;

typedef unsigned long DWORD;

typedef void* (*mono_thread_attach)(void* domain);
typedef void* (*mono_get_root_domain)();
typedef void* (*mono_assembly_open)(char* file, void* stat);
typedef void* (*mono_assembly_get_image)(void* assembly);
typedef void* (*mono_class_from_name)(void* image, char* namespacee, char* name);
typedef void* (*mono_class_get_method_from_name)(void* classs, char* name, DWORD param_count);
typedef void* (*mono_runtime_invoke)(void* method, void* instance, void* *params, void* exc);


mono_get_root_domain do_mono_get_root_domain;
mono_assembly_open do_mono_assembly_open;
mono_assembly_get_image do_mono_assembly_get_image;
mono_class_from_name do_mono_class_from_name;
mono_class_get_method_from_name do_mono_class_get_method_from_name;
mono_runtime_invoke do_mono_runtime_invoke;
mono_thread_attach do_mono_thread_attach;



int __attribute__((constructor)) monoloaderinit()
{
    ofstream myfile ("Client.log");

    if (myfile.is_open())
    {
 
        void* library = dlopen("./Armada_Data/MonoBleedingEdge/x86_64/libmonobdwgc-2.0.so",  RTLD_NOLOAD | RTLD_NOW);
     
        do_mono_thread_attach = (mono_thread_attach)(dlsym(library, "mono_thread_attach"));
        do_mono_get_root_domain = (mono_get_root_domain)(dlsym(library, "mono_get_root_domain"));
        do_mono_assembly_open = (mono_assembly_open)(dlsym(library, "mono_assembly_open"));
        do_mono_assembly_get_image = (mono_assembly_get_image)(dlsym(library, "mono_assembly_get_image"));
        do_mono_class_from_name = (mono_class_from_name)(dlsym(library, "mono_class_from_name"));
        do_mono_class_get_method_from_name = (mono_class_get_method_from_name)(dlsym(library, "mono_class_get_method_from_name"));
        do_mono_runtime_invoke = (mono_runtime_invoke)(dlsym(library, "mono_runtime_invoke"));

        myfile << "Started injection\n";

        // Init code //
        do_mono_thread_attach(do_mono_get_root_domain());
        myfile << "mono_get_root_domain\n";

        void* assembly = do_mono_assembly_open("./Armada_Data/Managed/Client.dll", NULL);
        myfile << "mono_assembly_open" << " " << assembly << endl;
        void* Image = do_mono_assembly_get_image(assembly);
        myfile << "mono_assembly_get_image" << " " << Image << endl ;
        void* MonoClass = do_mono_class_from_name(Image, "Test", "Test");
        myfile << "mono_class_from_name" << " " << MonoClass << endl;
        void* MonoClassMethod = do_mono_class_get_method_from_name(MonoClass, "Load", 0);
        myfile << "mono_class_get_method_from_name" << " " << MonoClassMethod << endl ;

        do_mono_runtime_invoke(MonoClassMethod, NULL, NULL, NULL);
        
        myfile << "Finished\n";
        myfile.close();
    }
    else cout << "Unable to open file";

    return 0;
}

void __attribute__((destructor)) monoloadershutdown()
{

};
